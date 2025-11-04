namespace EventBookingWeb.Models.DomainModels
{
    public class DBTicket
    {
        public required int TicketId { get; set; }
        public required int BookingID { get; set; }
        public required string UserID { get; set; }
        public required string TicketCode { get; set; }
        public required string QRCodeData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DBUser? User { get; set; }
        public DBBooking? Booking { get; set; }
    }
}
