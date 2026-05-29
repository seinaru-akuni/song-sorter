namespace SongSorterWebAPI.Models
{
    public class GoogleUser
    {
        public int Id { get; set; }

        // Унікальний ID, який прийде від Google (щоб ми завжди впізнавали користувача)
        public required string GoogleId { get; set; }

        public required string Email { get; set; }

        // Ось куди ми сховаємо наш довгостроковий ключ!
        public string? RefreshToken { get; set; }

        // Дата, коли токен перестане діяти (щоб знати, коли просити логін заново)
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
