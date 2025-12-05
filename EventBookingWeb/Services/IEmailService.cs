namespace EventBookingWeb.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendBookingConfirmationAsync(string toEmail, string userName, string eventTitle, string bookingCode, DateTime eventDate, int quantity, decimal totalAmount);
        Task SendTicketAsync(string toEmail, string userName, string eventTitle, string ticketCode, DateTime eventDate);
        Task SendPasswordResetAsync(string toEmail, string resetToken, string userName);
        Task SendBookingCancellationAsync(string toEmail, string userName, string eventTitle, string bookingCode);
    }
}

