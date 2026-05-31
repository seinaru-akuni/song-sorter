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
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        // Створюємо екземпляр хешера паролів
        private readonly PasswordHasher<AppUser> _passwordHasher = new();

        public AuthController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // ==========================================
        // 1. РЕЄСТРАЦІЯ
        // ==========================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            // Перевіряємо, чи співпадають паролі
            if (request.Password != request.ConfirmPassword)
                return BadRequest("Паролі не співпадають.");

            // Перевіряємо, чи не зайнята вже пошта або юзернейм
            if (await _userService.IsEmailTakenAsync(request.Email))
                return BadRequest("Користувач з таким Email вже існує.");

            if (await _userService.IsUsernameTakenAsync(request.Username))
                return BadRequest("Цей Username вже зайнятий.");

            // Створюємо нового користувача
            var newUser = new AppUser
            {
                Email = request.Email,
                Username = request.Username
            };

            // Хешуємо пароль (PasswordHasher автоматично генерує сіль)
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

            _userService.AddNewAppUser(newUser);
            await _userService.ContextSaveChangesAsync();

            // Відразу після реєстрації генеруємо сесію (логінимо користувача)
            _jwtService.GenerateAndSetTokenCookie(newUser.Id, HttpContext);

            return Ok(new { message = "Реєстрація успішна!" });
        }

        // ==========================================
        // 2. ЛОГІН (ВХІД)
        // ==========================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            // Шукаємо користувача за Email
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return Unauthorized("Невірний Email або пароль.");

            // Перевіряємо хеш пароля
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Невірний Email або пароль.");

            // Якщо пароль підходить, видаємо безпечну куку
            _jwtService.GenerateAndSetTokenCookie(user.Id, HttpContext);

            return Ok(new { message = "Вхід успішний!" });
        }

        // ==========================================
        // 1. ПЕРЕВІРКА СЕСІЇ ТА ОТРИМАННЯ ДАНИХ (Для React)
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
            var user = await _context.AppUsers
                .Include(u => u.LinkedAccounts) // Підтягуємо таблицю підключень
                .FirstOrDefaultAsync(u => u.Id == userId);

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
                // Показуємо фронтенду, які сервіси вже підключені (наприклад: ["Google", "Spotify"])
                connectedServices = user.LinkedAccounts.Select(la => la.ProviderName).ToList()
            });
        }

        // ==========================================
        // 2. ВИХІД З АКАУНТУ (LOGOUT)
        // ==========================================
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Щоб розлогінити користувача, нам достатньо просто наказати його 
            // браузеру видалити нашу безпечну куку
            Response.Cookies.Delete("jwt_token");

            return Ok(new { message = "Успішний вихід із системи" });
        }
    }
}
