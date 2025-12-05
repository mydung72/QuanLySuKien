namespace EventBookingWeb.Models.DomainModels
{
    public class DBPasswordReset
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}

