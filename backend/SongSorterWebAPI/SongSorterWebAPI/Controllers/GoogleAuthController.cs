using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using SongSorterWebAPI.Data;
using SongSorterWebAPI.Models;
using SongSorterWebAPI.Services;

namespace SongSorterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ITokenProtectionService _tokenProtection;

        public GoogleAuthController(
            IConfiguration configuration,
            AppDbContext context,
            ITokenProtectionService tokenProtection)
        {
            _configuration = configuration;
            _context = context;
            _tokenProtection = tokenProtection;
        }

        [HttpPost("callback")]
        public async Task<IActionResult> GoogleCallback([FromBody] AuthCodeDto request)
        {
            if (string.IsNullOrEmpty(request.AuthCode)) return BadRequest("Код відсутній.");

            var values = new Dictionary<string, string>
            {
                { "client_id", _configuration["Authentication:Google:ClientId"]! },
                { "client_secret", _configuration["Authentication:Google:ClientSecret"]! },
                { "code", request.AuthCode },
                { "grant_type", "authorization_code" },
                { "redirect_uri", "postmessage" }
            };

            using var client = new HttpClient();
            var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(values));
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, responseString);

            // 1. Розпаковуємо JSON від Google
            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseString);
            var idToken = tokenData.GetProperty("id_token").GetString();
            var refreshToken = tokenData.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;

            // 2. Читаємо Email та GoogleId з id_token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);

            var googleId = jwtToken.Claims.First(claim => claim.Type == "sub").Value;
            var email = jwtToken.Claims.First(claim => claim.Type == "email").Value;

            // 3. Шукаємо юзер  а в базі або створюємо нового
            var user = _context.GoogleUsers.FirstOrDefault(u => u.GoogleId == googleId);

            if (user == null)
            {
                user = new GoogleUser
                {
                    GoogleId = googleId,
                    Email = email
                };
                _context.GoogleUsers.Add(user);
            }

            // 4. Якщо Google прислав новий refresh_token, шифруємо і зберігаємо
            if (!string.IsNullOrEmpty(refreshToken))
            {
                user.RefreshToken = _tokenProtection.EncryptToken(refreshToken);
            }

            // Зберігаємо зміни у SQL Server
            await _context.SaveChangesAsync();

            // Віддаємо React-у відповідь, як і раніше
            return Content(responseString, "application/json");
        }

        // ... Метод RefreshToken залишається, ми оновимо його трохи пізніше ...

        public class AuthCodeDto { public required string AuthCode { get; set; } }
        public class RefreshDto { public required string RefreshToken { get; set; } }
    }
}