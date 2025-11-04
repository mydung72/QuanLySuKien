namespace EventBookingWeb.Models.DomainModels
{
    public class DBNews
    {
        public required int NewsId { get; set; }
        public required int EventId { get; set; }
        public required string Title { get; set; }
        public required string NewsContentHtml { get; set; }
        public required string ImageUrl { get; set; }
        public bool IsVisible { get; set; }
        public DateTime CreatedAt { get; set; }
        public DBEvent? Event { get; set; }
    }
}
