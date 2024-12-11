using BuffetAPI.Models.Accounts;
using Microsoft.AspNetCore.Identity;

namespace BuffetAPI.Auth
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> RegisterOgre(RegisterDto register);
        Task<IEnumerable<IdentityError>> RegisterCuisinier(RegisterDto register);
        Task<AuthResponse> Login(LoginDto login);

    }
}
