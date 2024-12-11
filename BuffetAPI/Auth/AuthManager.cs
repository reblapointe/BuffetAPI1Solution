using BuffetAPI.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuffetAPI.Auth
{
    public class AuthManager(UserManager<IdentityUser> userManager, IConfiguration configuration) : IAuthManager
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private IdentityUser _user;

        public async Task<AuthResponse> Login(LoginDto login)
        {
            _user = await _userManager.FindByNameAsync(login.Username);
            bool isValidUser = _user is not null
                    && await _userManager.CheckPasswordAsync(_user, login.Password);

            if (_user == null || !isValidUser)
            {
                return null;
            }

            var token = await GenerateToken();
            return new AuthResponse
            {
                Token = token,
                UserId = _user.Id
            };

        }

        public async Task<IEnumerable<IdentityError>> RegisterOgre(RegisterDto register)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = register.Username,
                Email = register.Email
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "OGRE");
            }
            return result.Errors;
        }


        public async Task<IEnumerable<IdentityError>> RegisterCuisinier(RegisterDto register)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = register.Username,
                Email = register.Email
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "CUISINIER");
            }
            return result.Errors;
        }


        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Name, _user.UserName),
                new (JwtRegisteredClaimNames.Sub, _user.UserName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Email, _user.Email),             
            }.Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToInt32(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
