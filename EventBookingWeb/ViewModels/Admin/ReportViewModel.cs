namespace EventBookingWeb.ViewModels.Admin
{
    public class ReportViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalAttendees { get; set; }
        public List<RevenueByDateViewModel> RevenueByDate { get; set; } = new();
        public List<EventAttendanceViewModel> EventAttendance { get; set; } = new();
        public List<PopularEventViewModel> PopularEvents { get; set; } = new();
    }

    public class RevenueByDateViewModel
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class EventAttendanceViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int TotalAttendees { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class PopularEventViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventImageUrl { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public int TotalAttendees { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

