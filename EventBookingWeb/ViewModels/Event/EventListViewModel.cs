using EventBookingWeb.Models.DomainModels;

namespace EventBookingWeb.ViewModels.Event
{
    public class EventListViewModel
    {
        public List<DBEvent> Events { get; set; } = new();
        public EventFilterViewModel Filter { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }
    }
}

