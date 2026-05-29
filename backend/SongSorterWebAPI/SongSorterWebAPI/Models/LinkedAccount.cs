namespace SongSorterWebAPI.Models
{
    public class LinkedAccount
    {
        public int Id { get; set; }
        
        // --- ЗВ'ЯЗОК З ГОЛОВНИМ ПРОФІЛЕМ ---
        // Зовнішній ключ (Foreign Key)
        public int AppUserId { get; set; } 
        // Навігаційна властивість для зручної роботи в C# коді
        public AppUser AppUser { get; set; } = null!; 

        // --- ДАНІ ЗОВНІШНЬОГО ПРОВАЙДЕРА ---
        
        // Назва сервісу: наприклад, "Google", "Spotify", "AppleMusic"
        public required string ProviderName { get; set; } 
        
        // Унікальний ID від самого провайдера (замінює колишній GoogleId)
        public required string ProviderAccountId { get; set; } 
        
        // Електронна пошта, яка прив'язана саме до цього зовнішнього акаунту
        public required string Email { get; set; } 
        
        // --- ДАНІ ДЛЯ АВТОРИЗАЦІЇ (API) ---
        
        // Зашифрований довгостроковий ключ доступу
        public string? RefreshToken { get; set; } 
        
        // Дата згоряння токену (для Google буде NULL, для інших — конкретна дата)
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
