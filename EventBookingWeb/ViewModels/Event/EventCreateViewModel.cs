using EventBookingWeb.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EventBookingWeb.ViewModels.Event
{
    public class EventCreateViewModel
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        public string? EventDescription { get; set; }

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string EventContentHtml { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa điểm là bắt buộc")]
        [StringLength(300, ErrorMessage = "Địa điểm không được vượt quá 300 ký tự")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Tổng số chỗ là bắt buộc")]
        [Range(1, 100000, ErrorMessage = "Số chỗ phải từ 1 đến 100000")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Giá vé là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá vé phải lớn hơn hoặc bằng 0")]
        public double TicketPrice { get; set; }

        public IFormFile? PosterImage { get; set; }
        
        public string? ExistingImageUrl { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public EventStatus EventStatus { get; set; }

        public List<int> SelectedCategoryIds { get; set; } = new();
    }
}

