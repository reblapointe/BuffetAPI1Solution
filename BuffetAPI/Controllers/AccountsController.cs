using BuffetAPI.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json.Linq;

namespace BuffetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController(IAuthManager authManager, ILogger<AccountsController> logger) : ControllerBase
    {
        private readonly IAuthManager _authManager = authManager;
        private readonly ILogger _logger = logger;

        // POST: api/Account/register-ogre
        [HttpPost]
        [Route("register-ogre")]
        public async Task<ActionResult> RegisterOgre([FromBody] Models.Accounts.RegisterDto register)
        {
            _logger.LogInformation("Tentative d'enregistrement d'un nouvel ogre pour {email}, {name}", register.Email, register.Username);
            var errors = await _authManager.RegisterOgre(register);
            if (errors.Any())
            {
                string s = string.Empty;
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    s += error.Code + " " + error.Description + ", ";
                }
                _logger.LogError("Tentative d'enregistrement d'un nouveau cuisinier pour {email}, {name} échouée : {errors}",
                    register.Email, register.Username, s);
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // POST: api/Account/register-cuisinier
        [HttpPost]
        [Route("register-cuisinier")]
        public async Task<ActionResult> RegisterCuisinier([FromBody] Models.Accounts.RegisterDto register)
        {
            _logger.LogInformation("Tentative d'enregistrement d'un nouveau cuisinier pour {email}, {name}", register.Email, register.Username);
            var errors = await _authManager.RegisterCuisinier(register);
            string s = string.Empty;
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    s += error.Code + " " + error.Description + ", ";
                }
                _logger.LogError("Tentative d'enregistrement d'un nouveau cuisinier pour {email}, {name} échouée : {errors}", 
                    register.Email, register.Username, s);

                return BadRequest(ModelState);
            }
            return Ok();
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] Models.Accounts.LoginDto login)
        {
            var authResponse = await _authManager.Login(login);
            if (authResponse is null)
            {
                _logger.LogWarning("Tentative de connexion échouée de {name}", login.Username);
                return Unauthorized();
            }
            _logger.LogInformation("Connexion réussie de {name}", login.Username);

            return Ok(authResponse);
        }
    }
}