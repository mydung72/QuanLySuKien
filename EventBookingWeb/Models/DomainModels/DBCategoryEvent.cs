namespace EventBookingWeb.Models.DomainModels
{
    public class DBCategoryEvent
    {
        public required int CategoryEventId { get; set; }
        public required string CategoryEventName { get; set; }
        public string? CategoryEventDescription { get; set; }
        public List<DBEvent>? Events { get; set; }
    }
}
