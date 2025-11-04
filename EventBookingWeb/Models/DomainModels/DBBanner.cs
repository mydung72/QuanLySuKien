namespace EventBookingWeb.Models.DomainModels
{
    public class DBBanner
    {
        public required int BannerID { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
