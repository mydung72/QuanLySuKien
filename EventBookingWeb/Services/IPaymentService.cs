namespace EventBookingWeb.Services
{
    public interface IPaymentService
    {
        string CreatePaymentUrl(int bookingId, decimal amount, string orderInfo, string returnUrl);
        bool ValidatePaymentCallback(Dictionary<string, string> queryParams);
        Task<bool> ProcessPaymentCallbackAsync(int bookingId, bool isSuccess, string transactionId);
    }
}

