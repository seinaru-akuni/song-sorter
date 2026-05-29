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

        // Навігаційна властивість для EF Core: 
        // Вказує, що у цього користувача може бути багато підключених інтеграцій
        public List<LinkedAccount> LinkedAccounts { get; set; } = new();
    }
}
