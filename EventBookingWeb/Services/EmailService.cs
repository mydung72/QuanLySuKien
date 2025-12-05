using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace EventBookingWeb.Services
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
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailSettings["FromName"], emailSettings["FromEmail"]));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"] ?? "587"), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string userName, string eventTitle, string bookingCode, DateTime eventDate, int quantity, decimal totalAmount)
        {
            var subject = $"Xác nhận đặt chỗ - {eventTitle}";
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Xác nhận đặt chỗ thành công</h2>
                    <p>Xin chào {userName},</p>
                    <p>Bạn đã đặt chỗ thành công cho sự kiện <strong>{eventTitle}</strong></p>
                    <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0;'>
                        <p><strong>Mã đặt chỗ:</strong> {bookingCode}</p>
                        <p><strong>Ngày sự kiện:</strong> {eventDate:dd/MM/yyyy HH:mm}</p>
                        <p><strong>Số lượng vé:</strong> {quantity}</p>
                        <p><strong>Tổng tiền:</strong> {totalAmount:N0} VNĐ</p>
                    </div>
                    <p>Vui lòng hoàn tất thanh toán để nhận vé điện tử.</p>
                    <p>Trân trọng,<br/>Event Booking Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendTicketAsync(string toEmail, string userName, string eventTitle, string ticketCode, DateTime eventDate)
        {
            var subject = $"Vé điện tử - {eventTitle}";
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Vé điện tử của bạn</h2>
                    <p>Xin chào {userName},</p>
                    <p>Đây là vé điện tử của bạn cho sự kiện <strong>{eventTitle}</strong></p>
                    <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0;'>
                        <p><strong>Mã vé:</strong> {ticketCode}</p>
                        <p><strong>Ngày sự kiện:</strong> {eventDate:dd/MM/yyyy HH:mm}</p>
                    </div>
                    <p>Vui lòng xuất trình mã vé này tại cổng vào sự kiện.</p>
                    <p>Trân trọng,<br/>Event Booking Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendPasswordResetAsync(string toEmail, string resetToken, string userName)
        {
            var subject = "Đặt lại mật khẩu";
            var resetUrl = $"{_configuration["AppUrl"]}/Account/ResetPassword?token={resetToken}&email={toEmail}";
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Đặt lại mật khẩu</h2>
                    <p>Xin chào {userName},</p>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng nhấn vào liên kết bên dưới để đặt lại mật khẩu:</p>
                    <p><a href='{resetUrl}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; display: inline-block;'>Đặt lại mật khẩu</a></p>
                    <p>Liên kết này sẽ hết hạn sau 24 giờ.</p>
                    <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                    <p>Trân trọng,<br/>Event Booking Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendBookingCancellationAsync(string toEmail, string userName, string eventTitle, string bookingCode)
        {
            var subject = $"Hủy đặt chỗ - {eventTitle}";
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Xác nhận hủy đặt chỗ</h2>
                    <p>Xin chào {userName},</p>
                    <p>Đặt chỗ của bạn cho sự kiện <strong>{eventTitle}</strong> đã được hủy thành công.</p>
                    <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0;'>
                        <p><strong>Mã đặt chỗ:</strong> {bookingCode}</p>
                    </div>
                    <p>Trân trọng,<br/>Event Booking Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }
    }
}

