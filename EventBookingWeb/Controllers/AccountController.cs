using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }




        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == username && u.PasswordHash == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            // Lưu thông tin đăng nhập vào Session
            HttpContext.Session.SetString("Username", user.FullName ?? "");
            HttpContext.Session.SetString("Role", user.Role.ToString());

            if (user.Role == UserRole.Admin)
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ViewBag.Error = "Email đã được sử dụng!";
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.FullName == username))
            {
                ViewBag.Error = "Tên người dùng đã tồn tại!";
                return View();
            }

            var newUser = new DBUser
            {
                UserId = 0,
                FullName = username,
                Email = email,
                PasswordHash = password, // Hoặc mã hoá nếu cần
                Role = UserRole.User,
                UserStatus = UserStatus.Active,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập.";
            return RedirectToAction("Login");
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
