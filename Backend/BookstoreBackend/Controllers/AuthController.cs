using BookstoreBackend.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookstoreBackend.DTOs.Auth;

namespace BookstoreBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { message = "Registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var userRoles = await _userManager.GetRolesAsync(user);


            // AuthController.cs

            var authClaims = new List<Claim>
{
    // --- СТАНДАРТНІ CLAIMS ДЛЯ ASP.NET CORE ---
    // Вони потрібні, щоб фреймворк і ваш код в контролерах працювали надійно.
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Email, user.Email),
    
    // --- ВАШІ ВЛАСНІ CLAIMS ---
    // Вони потрібні, щоб фронтенд міг легко читати дані.
    new Claim("id", user.Id),
    new Claim("firstName", user.FirstName),
    new Claim("lastName", user.LastName),

    // --- СЛУЖБОВІ CLAIMS ---
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

            foreach (var role in userRoles)
            {
                // Додаємо і стандартну роль, і вашу власну
                authClaims.Add(new Claim(ClaimTypes.Role, role));
                authClaims.Add(new Claim("role", role));
            }

            //            var authClaims = new List<Claim>
            //{
            //    //new Claim(ClaimTypes.NameIdentifier, user.Id),
            //    //new Claim(ClaimTypes.Email, user.Email),
            //    //new Claim(ClaimTypes.GivenName, user.FirstName), // Додаємо ім'я
            //    //new Claim(ClaimTypes.Surname, user.LastName),
            //        new Claim("id", user.Id),
            //        new Claim("email", user.Email),
            //        new Claim("firstName", user.FirstName),
            //        new Claim("lastName", user.LastName),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};

            //            foreach (var role in userRoles)
            //            {
            //                authClaims.Add(new Claim("role", role));
            //            }


            var token = GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            return new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}


