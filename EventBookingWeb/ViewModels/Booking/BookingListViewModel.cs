using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;

namespace EventBookingWeb.ViewModels.Booking
{
    public class BookingListViewModel
    {
        public List<BookingItemViewModel> Bookings { get; set; } = new();
        public PaymentStatus? FilterStatus { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
    }

    public class BookingItemViewModel
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventImageUrl { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public bool CanCancel { get; set; }
    }
}

