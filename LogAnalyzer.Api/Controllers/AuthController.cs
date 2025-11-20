using LogAnalyzer.Api.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginRequest = LogAnalyzer.Api.Models.LoginRequest;

namespace LogAnalyzer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, ILogger<AuthController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (request.Username != "admin" || request.Password != "password")
            {
                _logger.LogWarning("Invalid login attempt for user {user}", request.Username);
                return Unauthorized();
            }

            var token = GenerateJwtToken(request.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = request.Username
            });
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var keyValue = jwtSettings["Key"];

            Console.WriteLine("====== AUTH CONTROLLER DEBUG ======");
            Console.WriteLine("AuthController Key: " + keyValue);
            Console.WriteLine("AuthController Key Length: " + keyValue?.Length);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresMinutes"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
