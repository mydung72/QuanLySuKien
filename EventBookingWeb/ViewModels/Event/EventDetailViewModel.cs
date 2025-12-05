using EventBookingWeb.Models.DomainModels;

namespace EventBookingWeb.ViewModels.Event
{
    public class EventDetailViewModel
    {
        public DBEvent Event { get; set; } = null!;
        public List<DBCategoryEvent> Categories { get; set; } = new();
        public List<DBNews> RelatedNews { get; set; } = new();
        public bool CanBook { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}

