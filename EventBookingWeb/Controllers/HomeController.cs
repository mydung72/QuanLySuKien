using EventBookingWeb.Models;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using EventBookingWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EventBookingWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get active banners
                var banners = await _context.Banners
                    .Where(b => b.IsActive)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                // Get upcoming events
                var upcomingEvents = await _context.Events
                    .Include(e => e.CategoryEvents)
                    .Where(e => e.EventStatus == EventStatus.Upcoming && e.StartDate > DateTime.Now)
                    .OrderBy(e => e.StartDate)
                    .Take(6)
                    .ToListAsync();

                // Get latest news
                var latestNews = await _context.News
                    .Include(n => n.Event)
                    .Where(n => n.IsVisible)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(3)
                    .ToListAsync();

                ViewBag.Banners = banners;
                ViewBag.UpcomingEvents = upcomingEvents;
                ViewBag.LatestNews = latestNews;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading home page: {ex.Message}");
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Profile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString(),
                UserStatus = user.UserStatus.ToString(),
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Profile(UserProfileViewModel model)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            // ✅ Chỉ cập nhật các field cho phép sửa
            user.FullName = model.FullName;
            user.Phone = model.Phone;

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }
    }
}
