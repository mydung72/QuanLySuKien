using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;

namespace EventBookingWeb.ViewModels.Booking
{
    public class BookingDetailViewModel
    {
        public int BookingId { get; set; }
        public DBEvent Event { get; set; } = null!;
        public DBUser User { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public List<DBTicket> Tickets { get; set; } = new();
        public bool CanCancel { get; set; }
        public bool CanViewTickets { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string QRCodeBase64 { get; set; } = string.Empty;
    }
}

