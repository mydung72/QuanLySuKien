namespace EventBookingWeb.ViewModels.Ticket
{
    public class TicketListViewModel
    {
        public List<TicketItemViewModel> Tickets { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
    }

    public class TicketItemViewModel
    {
        public int TicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public string EventImageUrl { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

