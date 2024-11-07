using AuthorsWebApi.DTOs;
using AuthorsWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorsWebApi.Controllers.V2
{
    [ApiController]
    [Route("api/v2/accounts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public AccountsController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector(configuration["DataProtectionKey"]);
        }

        [HttpPost("create", Name = "createAccountV2")]
        public async Task<ActionResult<AccountCreationResponseDTO>> Create(UserCredentialsDTO userCredentials)
        {
            IdentityUser user = new IdentityUser()
            {
                UserName = userCredentials.Email,
                Email = userCredentials.Email
            };

            var creationResult = await userManager.CreateAsync(user, userCredentials.Password);

            if (creationResult.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(creationResult.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost("login", Name = "createLoginV2")]
        public async Task<ActionResult<AccountCreationResponseDTO>> Login(UserCredentialsDTO userCredentialsDTO)
        {
            var result = await signInManager.PasswordSignInAsync(
                userName: userCredentialsDTO.Email, password: userCredentialsDTO.Password,
                isPersistent: false, lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return await BuildToken(userCredentialsDTO);
            }

            return BadRequest("Incorrect login.");
        }

        [HttpGet("refresh-token", Name = "getRefreshTokenV2")]
        public async Task<ActionResult<AccountCreationResponseDTO>> RefreshToken()
        {
            Claim emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            if (emailClaim == null)
            {
                return NotFound("Please log in and try again.");
            }

            string email = emailClaim.Value;

            return await BuildToken(new UserCredentialsDTO
            {
                Email = email
            });

        }

        private async Task<AccountCreationResponseDTO> BuildToken(UserCredentialsDTO userCredentials)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("email",userCredentials.Email)
            };

            IdentityUser user = await userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDb = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDb);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddMinutes(30);

            var securityToken = new JwtSecurityToken(
                issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds
            );

            return new AccountCreationResponseDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration,
            };

        }

        [HttpPost("set-admin", Name = "createAdminV2")]
        public async Task<ActionResult> SetAdmin(UserAdminEditDTO userAdminEditDTO)
        {
            IdentityUser user = await userManager.FindByEmailAsync(userAdminEditDTO.Email);

            if (user == null)
            {
                return NotFound($"Don't exist a user with email {userAdminEditDTO.Email}.");
            }

            await userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }

        [HttpPost("remove-admin", Name = "deleteAdminV2")]
        public async Task<ActionResult> DeleteAdmin(UserAdminEditDTO userAdminEditDTO)
        {
            IdentityUser user = await userManager.FindByEmailAsync(userAdminEditDTO.Email);

            if (user == null)
            {
                return NotFound($"Don't exist a user with email {userAdminEditDTO.Email}.");
            }

            await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("encrypt-message", Name = "getEncryptMessageV2")]
        public ActionResult EncryptMessage(string message)
        {
            string encrypted = dataProtector.Protect(message);
            string dicrypted = dataProtector.Unprotect(encrypted);
            return Ok(new
            {
                plainText = message,
                encrypted,
                decrypted = dicrypted
            });
        }

        [AllowAnonymous]
        [HttpGet("encryption-time", Name = "getEncryptWithTimeV2")]
        public ActionResult EncryptWithTime(string message)
        {
            ITimeLimitedDataProtector dataProtectorTime = dataProtector.ToTimeLimitedDataProtector();
            string encrypted = dataProtectorTime.Protect(message, lifetime: TimeSpan.FromSeconds(4));
            Thread.Sleep(TimeSpan.FromSeconds(5));
            string decrypted = dataProtectorTime.Unprotect(encrypted);
            return Ok(new
            {
                message,
                encrypted,
                decrypted
            });
        }

        [AllowAnonymous]
        [HttpGet("hash-plain-text", Name = "getHashPlainTextV2")]
        public ActionResult HashPlainText(string plainText)
        {
            HashResultDTO hash1 = hashService.Hash(plainText);
            HashResultDTO hash2 = hashService.Hash(plainText);

            return Ok(new
            {
                plainText,
                hash1,
                hash2
            });

        }

    }
}
