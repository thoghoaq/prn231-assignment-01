using BusinessObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Linq;
using DataAccess;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DefaultAccount _defaultAccount;
        public readonly IAuthRepository _repository;

        public AuthController(IOptions<DefaultAccount> defaultAccount, IAuthRepository repository)
        {
            _defaultAccount = defaultAccount.Value;
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate the user credentials
            if (loginModel.Email == _defaultAccount.Email && loginModel.Password == _defaultAccount.Password)
            {
                return Ok(new UserClaims
                {
                    Role = _defaultAccount.Role,
                }
                );
            }
            else if (_repository.CheckUserInDatabase(loginModel) != null)
            {
                return Ok(new UserClaims
                {
                    Role = "User"
                });
            }
            else
            {
                return Unauthorized("Incorrect username or password");
            }
        }
    }
}
