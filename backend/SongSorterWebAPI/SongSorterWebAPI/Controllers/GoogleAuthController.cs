using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        [Authorize] // <--- ЗАХИСТ: викликати цей метод може тільки залогінений користувач
        public async Task<IActionResult> GoogleCallback([FromBody] AuthCodeDto request)
        {
            // 1. Дістаємо ID поточного користувача вашого додатку (AppUser) з JWT токена
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int currentAppUserId))
            {
                return Unauthorized(new { message = "Не вдалося ідентифікувати користувача." });
            }

            if (string.IsNullOrEmpty(request.AuthCode)) return BadRequest("Код відсутній.");

            // 2. Обмінюємо код на токени в Google
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

            // 3. Розпаковуємо відповідь Google
            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseString);
            var idToken = tokenData.GetProperty("id_token").GetString();
            var refreshToken = tokenData.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);

            var googleId = jwtToken.Claims.First(claim => claim.Type == "sub").Value;
            var googleEmail = jwtToken.Claims.First(claim => claim.Type == "email").Value;

            // ==============================================================
            // 4. НОВА ЛОГІКА ПРИВ'ЯЗКИ АКАУНТА (БЕЗ ПРИВ'ЯЗКИ ДО ПОШТИ)
            // ==============================================================

            // Перевіряємо, чи цей Google-акаунт ВЖЕ існує в нашій базі
            var existingLinkedAccount = await _context.LinkedAccounts
                .FirstOrDefaultAsync(la => la.ProviderName == "Google" && la.ProviderAccountId == googleId);

            if (existingLinkedAccount != null)
            {
                // Якщо він є, але прив'язаний до ІНШОГО користувача додатка
                if (existingLinkedAccount.AppUserId != currentAppUserId)
                {
                    return BadRequest(new { message = "Цей Google-акаунт вже прив'язаний до іншого профілю у системі." });
                }

                // Якщо він прив'язаний до ПОТОЧНОГО користувача (наприклад, юзер повторно дав дозвіл)
                // Просто оновлюємо RefreshToken, якщо Google видав новий
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    existingLinkedAccount.RefreshToken = _tokenProtection.EncryptToken(refreshToken);
                    await _context.SaveChangesAsync();
                }

                return Content(responseString, "application/json");
            }

            // 5. Якщо такого Google-акаунта ще немає в базі — створюємо зв'язок
            var newLinkedAccount = new LinkedAccount
            {
                AppUserId = currentAppUserId, // Прив'язуємо до ID з поточного JWT токена
                ProviderName = "Google",
                ProviderAccountId = googleId,
                Email = googleEmail           // Зберігаємо пошту від Google для історії/відображення (вона може відрізнятися від AppUser.Email)
            };

            if (!string.IsNullOrEmpty(refreshToken))
            {
                newLinkedAccount.RefreshToken = _tokenProtection.EncryptToken(refreshToken);
            }

            _context.LinkedAccounts.Add(newLinkedAccount);
            await _context.SaveChangesAsync();

            // Віддаємо React-у відповідь з токенами
            return Content(responseString, "application/json");
        }

        public class AuthCodeDto { public required string AuthCode { get; set; } }
        public class RefreshDto { public required string RefreshToken { get; set; } }
    }
}