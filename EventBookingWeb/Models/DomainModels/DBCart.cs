namespace EventBookingWeb.Models.DomainModels
{
    public class DBCart
    {
        public required int CartId { get; set; }
        public required int UserId { get; set; }
        public required int EventId { get; set; }
        public required int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public DBUser? User { get; set; }
        public DBEvent? Event { get; set; }
    }
}

