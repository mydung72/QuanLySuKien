using EventBookingWeb.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventBookingWeb.Models.DomainModels
{
    public class DBUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
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
