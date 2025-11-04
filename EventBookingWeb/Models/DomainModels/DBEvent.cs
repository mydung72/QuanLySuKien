using EventBookingWeb.Models.Enums;

namespace EventBookingWeb.Models.DomainModels
{
    public class DBEvent
    {
        public required int EventId { get; set; }
        public required List<int> EventCategoryId { get; set; }
        public required string Title { get; set; }
        public string? EventDescription { get; set; }
        public required string EventContentHtml { get; set; }
        public required string Location { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int TotalSeats { get; set; }
        public required int AvailableSeats { get; set; }
        public required double TicketPrice { get; set; }
        public required string ImageUrl { get; set; }
        public required EventStatus EventStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DBCategoryEvent>? CategoryEvents { get; set; }
        public List<DBNews>? NewsList { get; set; }
    }
}
