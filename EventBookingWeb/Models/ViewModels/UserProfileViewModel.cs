namespace EventBookingWeb.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string UserStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
