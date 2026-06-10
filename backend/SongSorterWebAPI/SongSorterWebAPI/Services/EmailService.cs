using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace SongSorterWebAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationCodeAsync(string toEmail, string code)
        {
            // 1. Створюємо каркас листа за допомогою MimeKit
            var emailMessage = new MimeMessage();

            // Від кого
            emailMessage.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"],
                _config["EmailSettings:SenderEmail"]
            ));

            // Кому
            emailMessage.To.Add(new MailboxAddress("", toEmail));

            // Тема листа
            emailMessage.Subject = "Код підтвердження реєстрації - Song Sorter";

            // Тіло листа (можна використовувати HTML-верстку)
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 10px; max-width: 500px;'>
                        <h2 style='color: #4CAF50;'>Ласкаво просимо до Song Sorter!</h2>
                        <p>Ваш код для підтвердження реєстрації:</p>
                        <div style='font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #333; background: #f4f4f4; padding: 10px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                            {code}
                        </div>
                        <p style='font-size: 12px; color: #777;'>Цей код дійсний протягом 15 хвилин. Якщо ви не реєструвалися на нашому сайті, просто проігноруйте цей лист.</p>
                    </div>"
            };

            // 2. Відправляємо лист за допомогою MailKit SmtpClient
            using var client = new SmtpClient();
            try
            {
                // Підключаємося до сервера Gmail (хост, порт, використання SSL/TLS)
                await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);

                // Авторизуємося за допомогою пароля додатка
                await client.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]);

                // Надсилаємо
                await client.SendAsync(emailMessage);
            }
            finally
            {
                // Обов'язково відключаємося від сервера після завершення
                await client.DisconnectAsync(true);
            }
        }
    }
}
