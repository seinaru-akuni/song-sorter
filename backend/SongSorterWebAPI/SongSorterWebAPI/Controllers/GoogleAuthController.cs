using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SongSorterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        // Цей метод ловитиме POST-запит від React
        [HttpPost("google-login")]
        public IActionResult GoogleLogin([FromBody] TokenDto request)
        {
            if (string.IsNullOrEmpty(request.AccessToken))
            {
                return BadRequest("Токен не отримано від фронтенду.");
            }

            // TODO: Тут ми будемо перевіряти токен через сервери Google
            // і діставати інфу про користувача (ім'я, email).

            // А поки що просто повертаємо успішну відповідь для тесту зв'язку:
            return Ok(new
            {
                message = "Бекенд успішно отримав токен доступу до YouTube!",
                receivedTokenPrefix = request.AccessToken.Substring(0, 15) + "..." // Показуємо шматочок токена для перевірки
            });
        }
        public class TokenDto
        {
            public required string AccessToken { get; set; }
        }

    }
}
