using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;

namespace EventBookingWeb.ViewModels.Admin
{
    public class UserManagementViewModel
    {
        public List<DBUser> Users { get; set; } = new();
        public UserRole? FilterRole { get; set; }
        public UserStatus? FilterStatus { get; set; }
        public string? SearchTerm { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
    }
}

