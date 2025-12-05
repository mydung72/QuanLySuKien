using EventBookingWeb.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace EventBookingWeb.ViewModels.Admin
{
    public class UserEditViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public UserStatus UserStatus { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}

