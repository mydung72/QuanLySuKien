using EventBookingWeb.Models.Enums;

namespace EventBookingWeb.ViewModels.Admin
{
    public class BookingManagementViewModel
    {
        public List<BookingManagementItemViewModel> Bookings { get; set; } = new();
        public int? FilterEventId { get; set; }
        public PaymentStatus? FilterStatus { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public string? SearchTerm { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
    }

    public class BookingManagementItemViewModel
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
    }
}

