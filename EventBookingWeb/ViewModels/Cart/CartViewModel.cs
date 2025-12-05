using EventBookingWeb.Models.DomainModels;

namespace EventBookingWeb.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }

    public class CartItemViewModel
    {
        public int CartId { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventImageUrl { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public double TicketPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => (decimal)(Quantity * TicketPrice);
        public int AvailableSeats { get; set; }
    }
}

