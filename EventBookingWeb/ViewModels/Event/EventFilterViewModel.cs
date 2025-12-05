namespace EventBookingWeb.ViewModels.Event
{
    public class EventFilterViewModel
    {
        public int? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }
}

