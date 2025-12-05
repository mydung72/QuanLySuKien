using EventBookingWeb.Attributes;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using EventBookingWeb.Services;
using EventBookingWeb.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers.Admin
{
    [AuthorizeAdmin]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(
            ApplicationDbContext context,
            IReportService reportService,
            ILogger<ReportController> logger)
        {
            _context = context;
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IActionResult> Revenue(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.Event)
                    .Where(b => b.PaymentStatus == PaymentStatus.Paid);

                if (startDate.HasValue)
                    query = query.Where(b => b.BookingDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(b => b.BookingDate <= endDate.Value);

                var bookings = await query.ToListAsync();

                var revenueByDate = bookings
                    .GroupBy(b => b.BookingDate.Date)
                    .Select(g => new RevenueByDateViewModel
                    {
                        Date = g.Key,
                        Revenue = g.Sum(b => b.TotalAmount),
                        BookingCount = g.Count()
                    })
                    .OrderBy(r => r.Date)
                    .ToList();

                var viewModel = new ReportViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalRevenue = bookings.Sum(b => b.TotalAmount),
                    TotalBookings = bookings.Count,
                    TotalAttendees = bookings.Sum(b => b.Quantity),
                    RevenueByDate = revenueByDate
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating revenue report: {ex.Message}");
                return View(new ReportViewModel());
            }
        }

        public async Task<IActionResult> Attendance()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.NewsList)
                    .ToListAsync();

                var eventAttendance = new List<EventAttendanceViewModel>();

                foreach (var eventItem in events)
                {
                    var bookings = await _context.Bookings
                        .Where(b => b.EventId == eventItem.EventId && b.PaymentStatus == PaymentStatus.Paid)
                        .ToListAsync();

                    eventAttendance.Add(new EventAttendanceViewModel
                    {
                        EventId = eventItem.EventId,
                        EventTitle = eventItem.Title,
                        TotalAttendees = bookings.Sum(b => b.Quantity),
                        TotalBookings = bookings.Count,
                        TotalRevenue = bookings.Sum(b => b.TotalAmount)
                    });
                }

                var viewModel = new ReportViewModel
                {
                    EventAttendance = eventAttendance.OrderByDescending(e => e.TotalAttendees).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating attendance report: {ex.Message}");
                return View(new ReportViewModel());
            }
        }

        public async Task<IActionResult> PopularEvents()
        {
            try
            {
                var events = await _context.Events.ToListAsync();

                var popularEvents = new List<PopularEventViewModel>();

                foreach (var eventItem in events)
                {
                    var bookings = await _context.Bookings
                        .Where(b => b.EventId == eventItem.EventId && b.PaymentStatus == PaymentStatus.Paid)
                        .ToListAsync();

                    popularEvents.Add(new PopularEventViewModel
                    {
                        EventId = eventItem.EventId,
                        EventTitle = eventItem.Title,
                        EventImageUrl = eventItem.ImageUrl,
                        BookingCount = bookings.Count,
                        TotalAttendees = bookings.Sum(b => b.Quantity),
                        TotalRevenue = bookings.Sum(b => b.TotalAmount)
                    });
                }

                var viewModel = new ReportViewModel
                {
                    PopularEvents = popularEvents.OrderByDescending(e => e.BookingCount).Take(10).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating popular events report: {ex.Message}");
                return View(new ReportViewModel());
            }
        }

        public async Task<IActionResult> ExportExcel(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var excelBytes = await _reportService.ExportRevenueToExcelAsync(startDate, endDate);
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting to Excel: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi xuất báo cáo.";
                return RedirectToAction("Revenue");
            }
        }
    }
}

