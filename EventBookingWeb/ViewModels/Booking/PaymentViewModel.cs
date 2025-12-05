using System.ComponentModel.DataAnnotations;

namespace EventBookingWeb.ViewModels.Booking
{
    public class PaymentViewModel
    {
        [Required]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        // Display info
        public string? EventTitle { get; set; }
        public int Quantity { get; set; }
        public DateTime? EventDate { get; set; }
    }
}

