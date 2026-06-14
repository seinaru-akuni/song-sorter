using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SongSorterWebAPI.Data;
using System.Security.Claims;

using SongSorterWebAPI.Models;
using SongSorterWebAPI.Services;
using SongSorterWebAPI.DTOs;

namespace SongSorterWebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        // Створюємо екземпляр хешера паролів
        private readonly PasswordHasher<AppUser> _passwordHasher = new();

        public AuthController(IJwtService jwtService, IUserService userService, IEmailService emailService)
        {
            _jwtService = jwtService;
            _userService = userService;
            _emailService = emailService;
        }

        // ==========================================
        // 1. РЕЄСТРАЦІЯ
        // ==========================================


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (request.Password != request.ConfirmPassword)
                return BadRequest("Паролі не співпадають.");

            if (await _userService.IsEmailTakenAsync(request.Email))
                return BadRequest("Користувач з таким Email вже існує.");

            if (await _userService.IsUsernameTakenAsync(request.Username))
                return BadRequest("Цей Username вже зайнятий.");

            string verificationCode = Random.Shared.Next(0, 1000000).ToString("D6");

            var newUser = new AppUser
            {
                Email = request.Email,
                Username = request.Username,
                IsEmailVerified = false,
                VerificationCode = verificationCode,
                VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15)
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

            _userService.AddNewAppUser(newUser);
            await _userService.ContextSaveChangesAsync();

            // !!! КУКУ ТУТ БІЛЬШЕ НЕ ВИДАЄМО. Користувач залогіниться через verify-email
            // await _emailService.SendVerificationCodeAsync(newUser.Email, verificationCode);

            return Ok(new { message = "Реєстрація успішна! Код підтвердження відправлено." });
        }


        // ==========================================
        // 2. ЛОГІН (ВХІД)
        // ==========================================


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            // Шукаємо користувача за Email
            var user = await _userService.FindUserViaEmailAsync(request.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized("Невірний Email або пароль.");

            // Перевіряємо хеш пароля
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Невірний Email або пароль.");

            // Якщо пароль підходить, видаємо безпечну куку
            _jwtService.GenerateAndSetTokenCookie(user.Id, HttpContext, request.RememberMe);

            return Ok(new { message = "Вхід успішний!" });
        }


        // ==========================================
        // 3. ВЕРИФІКААЦІЯ ЕМЕЙЛУ
        // ==========================================


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
        {
            var user = await _userService.FindUserViaEmailAsync(request.Email);

            if (user == null)
                return NotFound("Користувача не знайдено.");

            // Загальна перевірка коду для обох випадків
            if (user.VerificationCode != request.Code)
                return BadRequest("Невірний код.");

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
                return BadRequest("Термін дії коду минув.");

            // МАГІЯ: Перевіряємо, чи це флоу скидання пароля (прийшов новий пароль?)
            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                if (request.NewPassword != request.ConfirmNewPassword)
                    return BadRequest("Паролі не співпадають.");

                // Хешуємо та оновлюємо пароль на новий
                user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
                user.IsEmailVerified = true; // Якщо міняє пароль через пошту, то вона автоматично верифікована
            }
            else
            {
                // Якщо пароля немає — це звичайне підтвердження реєстрації
                user.IsEmailVerified = true;
            }

            // В обох випадках зачищаємо тимчасовий код
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;

            await _userService.ContextSaveChangesAsync();

            // Видаємо куку авторизації (UX-бонус: користувач одразу залогінений в обох сценаріях!)
            _jwtService.GenerateAndSetTokenCookie(user.Id, HttpContext, false);

            string responseMessage = !string.IsNullOrEmpty(request.NewPassword)
                ? "Пароль успішно змінено! Ви увійшли в систему."
                : "Пошту успішно підтверджено! Ви увійшли в систему.";

            return Ok(new { message = responseMessage });
        }



        // ==========================================
        // 4. ПЕРЕВІРКА СЕСІЇ ТА ОТРИМАННЯ ДАНИХ (Для React)
        // ==========================================



        [HttpGet("me")]
        [Authorize] // <--- Цей атрибут блокує запит, якщо куки немає або вона протухла!
        public async Task<IActionResult> GetCurrentUser()
        {
            // Якщо код дійшов сюди, значить .NET вже перевірив JWT куку 
            // і розшифрував дані. Дістаємо ID користувача (який ми ховали в Sub):
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            // Йдемо в базу і дістаємо профіль + інформацію про його підключення
            var user = await _userService.FindUserViaIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Повертаємо React-у тільки БЕЗПЕЧНІ дані.
            // Ніколи не відправляємо зашифровані токени чи хеші на фронтенд!
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                username = user.Username
            });
        }



        // ==========================================
        // 5. ЗМІНИТИ ПАРОЛЬ
        // ==========================================


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var user = await _userService.FindUserViaEmailAsync(request.Email);
            if (user == null)
                return NotFound("Користувача з таким Email не знайдено.");

            // Генеруємо новий код для зміни пароля
            string verificationCode = Random.Shared.Next(0, 1000000).ToString("D6");

            user.VerificationCode = verificationCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _userService.ContextSaveChangesAsync();

            // Відправляємо цей код на пошту
            // await _emailService.SendVerificationCodeAsync(user.Email, verificationCode);

            return Ok(new { message = "Код для відновлення пароля надіслано на пошту." });
        }


        // ==========================================
        // 6. ВИХІД З АКАУНТУ (LOGOUT)
        // ==========================================



        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Створюємо такі самі правила, які використовували при генерації
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            // Видаляємо куку, вказуючи ці правила
            Response.Cookies.Delete("jwt_token", cookieOptions);

            return Ok(new { message = "Успішний вихід із системи" });
        }
    }
}
