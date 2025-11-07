using EventBookingWeb.Models.Enums;
using Microsoft.AspNetCore.Identity;


namespace EventBookingWeb.Models.DomainModels
{
    public class DBUser
    {
        public required int UserId { get; set; }
        public string? FullName { get; set; }
        public required string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? Phone { get; set; }
        public required UserRole Role { get; set; }
        public required UserStatus UserStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DBBooking>? Bookings { get; set; }
        public List<DBTicket>? Tickets { get; set; }
    }
}
