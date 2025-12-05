using System.ComponentModel.DataAnnotations;

namespace EventBookingWeb.ViewModels.Booking
{
    public class BookingCreateViewModel
    {
        [Required]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Số lượng vé là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Số lượng vé phải từ 1 đến 100")]
        public int Quantity { get; set; }

        public string? AttendeeNotes { get; set; }

        // Display info
        public string? EventTitle { get; set; }
        public double TicketPrice { get; set; }
        public int AvailableSeats { get; set; }
        public decimal TotalAmount => (decimal)(Quantity * TicketPrice);
    }
}

