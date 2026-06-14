namespace SongSorterWebAPI.DTOs
{
    public class VerifyEmailDto
    {
        public required string Email { get; set; }
        public required string Code { get; set; }

        // Ці поля заповнюються ТІЛЬКИ при скиданні пароля
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
