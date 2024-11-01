using AuthorsWebApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountsController:ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountsController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<ActionResult<AccountCreationResponseDTO>> Create(UserCredentialsDTO userCredentials)
        {
            IdentityUser user = new IdentityUser() {
                UserName=userCredentials.Email,
                Email = userCredentials.Email 
            };

            var creationResult = await userManager.CreateAsync(user, userCredentials.Password);

            if (creationResult.Succeeded) {
                return BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(creationResult.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AccountCreationResponseDTO>> Login(UserCredentialsDTO userCredentialsDTO)
        {
            var result = await signInManager.PasswordSignInAsync(
                userName: userCredentialsDTO.Email, password: userCredentialsDTO.Password, 
                isPersistent:false, lockoutOnFailure: false
            );

            if (result.Succeeded) {
                return BuildToken(userCredentialsDTO);
            }

            return BadRequest("Incorrect login.");
        }
        
        private AccountCreationResponseDTO BuildToken(UserCredentialsDTO userCredentials)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("email",userCredentials.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddMinutes(2);

            var securityToken = new JwtSecurityToken(
                issuer: null, audience: null, claims:claims, expires: expiration, signingCredentials: creds
            );

            return new AccountCreationResponseDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration,
            };

        }
    }
}
