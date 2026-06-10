namespace SongSorterWebAPI.Services
{
    public interface IEmaleService
    {
        Task SendVerificationCodeAsync(string toEmail, string code);
    }
}
