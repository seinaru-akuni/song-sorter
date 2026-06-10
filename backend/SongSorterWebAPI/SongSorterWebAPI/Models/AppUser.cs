namespace SongSorterWebAPI.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        public string Username {  get; set; }

        // Головна електронна пошта для входу на твій сайт
        public required string Email { get; set; }

        // Пароль для майбутньої локальної реєстрації (поки що буде NULL)
        public string? PasswordHash { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }
        public List<LinkedAccount> LinkedAccounts { get; set; } = new();
    }
}
