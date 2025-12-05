using Microsoft.AspNetCore.Mvc;

namespace EventBookingWeb.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            // Kiểm tra quyền truy cập
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult Users() => View();
        public IActionResult Events() => View();
        public IActionResult Bookings() => View();
        public IActionResult MediaActivities() => View();
        public IActionResult Reports() => View();
    }
}
