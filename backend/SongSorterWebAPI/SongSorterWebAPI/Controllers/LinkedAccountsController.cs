using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongSorterWebAPI.Models;
using SongSorterWebAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace SongSorterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkedAccountsController : ControllerBase
    {
        private readonly ILinkedAccountService _linkedAccountService;

        public LinkedAccountsController(ILinkedAccountService linkedAccountService)
        {
            _linkedAccountService = linkedAccountService;
        }

        [HttpGet("get-list")]
        [Authorize]
        public async Task<IActionResult> GetLinkedAccounts()
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out int currentAppUserId))
            {
                return Unauthorized(new { message = "Не вдалося ідентифікувати користувача." });
            }

            var linkedAccounts = await _linkedAccountService.GetUserLinkedAccountsListAsync(currentAppUserId);

            var responseDto = linkedAccounts.Select(la => new
            {
                la.Id,
                la.ProviderName,
                la.Email,
                // la.ProviderAccountId // Можна додати, якщо фронтенду треба унікальний ID з Google/Spotify
            });

            // Ok() самостійно перетворить responseDto на правильний JSON
            return Ok(responseDto);
        }

    }
}
