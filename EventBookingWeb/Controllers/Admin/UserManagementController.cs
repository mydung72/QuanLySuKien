using EventBookingWeb.Attributes;
using EventBookingWeb.Helpers;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using EventBookingWeb.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers.Admin
{
    [AuthorizeAdmin]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(ApplicationDbContext context, ILogger<UserManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(UserRole? filterRole, UserStatus? filterStatus, string? searchTerm, int page = 1)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (filterRole.HasValue)
                    query = query.Where(u => u.Role == filterRole.Value);

                if (filterStatus.HasValue)
                    query = query.Where(u => u.UserStatus == filterStatus.Value);

                if (!string.IsNullOrEmpty(searchTerm))
                    query = query.Where(u => u.FullName != null && u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm));

                var pageSize = 20;
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var users = await query
                    .OrderBy(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new UserManagementViewModel
                {
                    Users = users,
                    FilterRole = filterRole,
                    FilterStatus = filterStatus,
                    SearchTerm = searchTerm,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading users: {ex.Message}");
                return View(new UserManagementViewModel());
            }
        }

        public IActionResult Create()
        {
            return View(new UserCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    return View(model);
                }

                var user = new DBUser
                {
                    UserId = 0,
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = PasswordHelper.HashPassword(model.Password),
                    Phone = model.Phone,
                    Role = model.Role,
                    UserStatus = model.UserStatus,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm người dùng thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var viewModel = new UserEditViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName ?? "",
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                UserStatus = user.UserStatus
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _context.Users.FindAsync(model.UserId);
                if (user == null)
                    return NotFound();

                if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.UserId != model.UserId))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    return View(model);
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Role = model.Role;
                user.UserStatus = model.UserStatus;

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    user.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật người dùng thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound();

                user.UserStatus = UserStatus.Locked;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Khóa tài khoản thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error locking user: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound();

                user.UserStatus = UserStatus.Active;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Mở khóa tài khoản thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error unlocking user: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Bookings)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                    return NotFound();

                if (user.Bookings != null && user.Bookings.Any())
                {
                    TempData["Error"] = "Không thể xóa người dùng đang có đặt chỗ";
                    return RedirectToAction("Index");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa người dùng thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }
    }
}

