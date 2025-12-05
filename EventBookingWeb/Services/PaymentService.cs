using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EventBookingWeb.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IConfiguration configuration, ApplicationDbContext context, ILogger<PaymentService> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public string CreatePaymentUrl(int bookingId, decimal amount, string orderInfo, string returnUrl)
        {
            try
            {
                var vnpaySettings = _configuration.GetSection("VNPaySettings");
                var vnp_TmnCode = vnpaySettings["TmnCode"];
                var vnp_HashSecret = vnpaySettings["HashSecret"];
                var vnp_Url = vnpaySettings["Url"];
                var vnp_ReturnUrl = returnUrl;

                var vnpay = new Dictionary<string, string>
                {
                    { "vnp_Version", "2.1.0" },
                    { "vnp_Command", "pay" },
                    { "vnp_TmnCode", vnp_TmnCode ?? "" },
                    { "vnp_Amount", ((int)(amount * 100)).ToString() },
                    { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                    { "vnp_CurrCode", "VND" },
                    { "vnp_IpAddr", "127.0.0.1" },
                    { "vnp_Locale", "vn" },
                    { "vnp_OrderInfo", orderInfo },
                    { "vnp_OrderType", "other" },
                    { "vnp_ReturnUrl", vnp_ReturnUrl },
                    { "vnp_TxnRef", bookingId.ToString() }
                };

                var sortedParams = vnpay.OrderBy(x => x.Key).ToList();
                var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
                var signData = string.Join("&", sortedParams.Select(x => $"{x.Key}={x.Value}"));
                var vnp_SecureHash = HmacSHA512(vnp_HashSecret ?? "", signData);

                return $"{vnp_Url}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating payment URL: {ex.Message}");
                return string.Empty;
            }
        }

        public bool ValidatePaymentCallback(Dictionary<string, string> queryParams)
        {
            try
            {
                var vnpaySettings = _configuration.GetSection("VNPaySettings");
                var vnp_HashSecret = vnpaySettings["HashSecret"];

                if (!queryParams.ContainsKey("vnp_SecureHash"))
                    return false;

                var vnp_SecureHash = queryParams["vnp_SecureHash"];
                queryParams.Remove("vnp_SecureHash");

                var sortedParams = queryParams.OrderBy(x => x.Key).ToList();
                var signData = string.Join("&", sortedParams.Select(x => $"{x.Key}={x.Value}"));
                var checkSum = HmacSHA512(vnp_HashSecret ?? "", signData);

                return checkSum.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating payment callback: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ProcessPaymentCallbackAsync(int bookingId, bool isSuccess, string transactionId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                    return false;

                if (isSuccess)
                {
                    booking.PaymentStatus = PaymentStatus.Paid;
                }
                else
                {
                    booking.PaymentStatus = PaymentStatus.Cancelled;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing payment callback: {ex.Message}");
                return false;
            }
        }

        private string HmacSHA512(string key, string data)
        {
            var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}

