using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MrRobotRoomChat.Request;

namespace MrRobotRoomChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : SecuredController
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _configuration = configuration;
            _logger = logger;

        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<UserController> _logger;

        [HttpGet]
        [Route("Test")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> LoguinAsync(UserRequest request)
        {
            var user = request.Username == "Demian" ? new User
            {
                Username = "Demian",
                Password = "1234"
            } : null;

            if (user != null && user.Password == user.Password)
            {
                TokenResponse tokenResponse = GenerateJwtToken(user);

                return Ok(tokenResponse);
            }

            return Unauthorized();
        }

        private TokenResponse? GenerateJwtToken(User user)
        {
            try
            {
                // generate token that is valid for 7 days
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("BFE2D374-6D48-45AB-BEA2-179CA3211077");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return new TokenResponse { Token = tokenHandler.WriteToken(token), ValidTo = token.ValidTo };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class TokenResponse
        {
            public string Token { get; set; }
            public DateTime ValidTo { get; set; }
        }
    }
}