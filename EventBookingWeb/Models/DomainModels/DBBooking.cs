using EventBookingWeb.Models.Enums;


namespace EventBookingWeb.Models.DomainModels
{
    public class DBBooking
    {
        public required int BookingId { get; set; }
        public required int EventId { get; set; }
        public required int UserId { get; set; }
        public required int Quantity { get; set; }
        public required decimal TotalAmount { get; set; }
        public required PaymentStatus PaymentStatus { get; set; }
        public required string PaymentMethod { get; set; }
        public DateTime BookingDate { get; set; }
        public DBUser? User { get; set; }
        public List<DBTicket>? Tickets { get; set; }
    }
}
