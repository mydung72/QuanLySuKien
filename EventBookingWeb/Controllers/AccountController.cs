using EventBookingWeb.Helpers;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using EventBookingWeb.Services;
using EventBookingWeb.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ApplicationDbContext context, 
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Redirect if already logged in
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                var role = HttpContext.Session.GetString("Role");
                if (role == UserRole.Admin.ToString())
                    return RedirectToAction("Dashboard", "Admin");
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng!");
                    return View(model);
                }

                if (user.UserStatus == UserStatus.Locked)
                {
                    ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa!");
                    return View(model);
                }

                // Save to session
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Username", user.FullName ?? "");
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role.ToString());

                if (user.Role == UserRole.Admin)
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng!");
                    return View(model);
                }

                var newUser = new DBUser
                {
                    UserId = 0,
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = PasswordHelper.HashPassword(model.Password),
                    Phone = model.Phone,
                    Role = UserRole.User,
                    UserStatus = UserStatus.Active,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // Don't reveal that user doesn't exist
                    TempData["Success"] = "Nếu email tồn tại, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu.";
                    return RedirectToAction("Login");
                }

                var resetToken = PasswordHelper.GenerateRandomToken();
                // In a real application, store this token in database with expiry
                
                await _emailService.SendPasswordResetAsync(user.Email, resetToken, user.FullName ?? "");

                TempData["Success"] = "Đã gửi email hướng dẫn đặt lại mật khẩu.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Forgot password error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        // GET: /Account/ResetPassword
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy người dùng.");
                    return View(model);
                }

                // In a real application, validate the token here

                user.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reset password error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
