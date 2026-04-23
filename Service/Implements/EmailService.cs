using BookStore.Service.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace BookStore.Service.Implements
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                emailSettings["FromName"] ?? "BookStore",
                emailSettings["FromEmail"] ?? ""));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            try
            {
                using var client = new SmtpClient();

                // Kết nối với STARTTLS (port 587)
                var smtpServer = emailSettings["SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");

                await client.ConnectAsync(
                    smtpServer,
                    smtpPort,
                    MailKit.Security.SecureSocketOptions.StartTls);

                // Xác thực
                var smtpUsername = emailSettings["SmtpUsername"] ?? "";
                var smtpPassword = emailSettings["SmtpPassword"] ?? "";

                await client.AuthenticateAsync(smtpUsername, smtpPassword);

                // Gửi email
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var subject = "Đặt lại mật khẩu - BookStore";
            var htmlBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Yêu cầu đặt lại mật khẩu</h2>
                    <p>Xin chào,</p>
                    <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                    <p>Nhấp vào nút bên dưới để đặt lại mật khẩu:</p>
                    <p style='text-align: center;'>
                        <a href='{resetLink}'
                           style='display: inline-block; padding: 12px 30px;
                                  background-color: #1677ff; color: white;
                                  text-decoration: none; border-radius: 6px;'>
                            Đặt lại mật khẩu
                        </a>
                    </p>
                        Liên kết này sẽ hết hạn sau 24 giờ.<br/>
                        Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.
                    </p>
                </div>
            </body>
            </html>
        ";

            await SendEmailAsync(email, subject, htmlBody);
        }
    }
}
