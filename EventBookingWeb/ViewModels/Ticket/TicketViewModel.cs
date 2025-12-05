using EventBookingWeb.Models.DomainModels;

namespace EventBookingWeb.ViewModels.Ticket
{
    public class TicketViewModel
    {
        public int TicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string QRCodeData { get; set; } = string.Empty;
        public string QRCodeBase64 { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DBEvent Event { get; set; } = null!;
        public DBBooking Booking { get; set; } = null!;
        public DBUser User { get; set; } = null!;
    }
}

