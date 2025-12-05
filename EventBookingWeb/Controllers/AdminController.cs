using EventBookingWeb.Attributes;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers.Admin
{
    [AuthorizeAdmin]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Statistics for dashboard
                var totalUsers = await _context.Users.CountAsync(u => u.Role == UserRole.User);
                var totalEvents = await _context.Events.CountAsync();
                var totalBookings = await _context.Bookings.CountAsync();
                var totalRevenue = await _context.Bookings
                    .Where(b => b.PaymentStatus == PaymentStatus.Paid)
                    .SumAsync(b => b.TotalAmount);

                // Recent bookings
                var recentBookings = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.User)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToListAsync();

                // Upcoming events
                var upcomingEvents = await _context.Events
                    .Where(e => e.StartDate > DateTime.Now)
                    .OrderBy(e => e.StartDate)
                    .Take(5)
                    .ToListAsync();

                // Statistics by month (current year)
                var currentYear = DateTime.Now.Year;
                var monthlyStats = await _context.Bookings
                    .Where(b => b.BookingDate.Year == currentYear && b.PaymentStatus == PaymentStatus.Paid)
                    .GroupBy(b => b.BookingDate.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Revenue = g.Sum(b => b.TotalAmount),
                        Count = g.Count()
                    })
                    .ToListAsync();

                ViewBag.TotalUsers = totalUsers;
                ViewBag.TotalEvents = totalEvents;
                ViewBag.TotalBookings = totalBookings;
                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.RecentBookings = recentBookings;
                ViewBag.UpcomingEvents = upcomingEvents;
                ViewBag.MonthlyStats = monthlyStats;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading dashboard: {ex.Message}");
                return View();
            }
        }
    }
}
