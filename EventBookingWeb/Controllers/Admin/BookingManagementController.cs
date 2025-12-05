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
    public class BookingManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingManagementController> _logger;

        public BookingManagementController(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<BookingManagementController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? filterEventId, PaymentStatus? filterStatus, DateTime? filterStartDate, DateTime? filterEndDate, string? searchTerm, int page = 1)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.User)
                    .AsQueryable();

                if (filterEventId.HasValue)
                    query = query.Where(b => b.EventId == filterEventId.Value);

                if (filterStatus.HasValue)
                    query = query.Where(b => b.PaymentStatus == filterStatus.Value);

                if (filterStartDate.HasValue)
                    query = query.Where(b => b.BookingDate >= filterStartDate.Value);

                if (filterEndDate.HasValue)
                    query = query.Where(b => b.BookingDate <= filterEndDate.Value);

                if (!string.IsNullOrEmpty(searchTerm))
                    query = query.Where(b => b.User != null && (b.User.FullName != null && b.User.FullName.Contains(searchTerm) || b.User.Email.Contains(searchTerm)));

                var pageSize = 20;
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var bookings = await query
                    .OrderByDescending(b => b.BookingDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new BookingManagementViewModel
                {
                    Bookings = bookings.Select(b => new BookingManagementItemViewModel
                    {
                        BookingId = b.BookingId,
                        EventId = b.EventId,
                        EventTitle = b.Event?.Title ?? "",
                        UserName = b.User?.FullName ?? "",
                        UserEmail = b.User?.Email ?? "",
                        Quantity = b.Quantity,
                        TotalAmount = b.TotalAmount,
                        PaymentStatus = b.PaymentStatus,
                        PaymentMethod = b.PaymentMethod,
                        BookingDate = b.BookingDate
                    }).ToList(),
                    FilterEventId = filterEventId,
                    FilterStatus = filterStatus,
                    FilterStartDate = filterStartDate,
                    FilterEndDate = filterEndDate,
                    SearchTerm = searchTerm,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize
                };

                ViewBag.Events = await _context.Events.ToListAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading bookings: {ex.Message}");
                return View(new BookingManagementViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.User)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                    return NotFound();

                booking.PaymentStatus = PaymentStatus.Paid;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Duyệt đặt chỗ thành công";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error approving booking: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Details", new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                    return NotFound();

                booking.PaymentStatus = PaymentStatus.Cancelled;
                if (booking.Event != null)
                {
                    booking.Event.AvailableSeats += booking.Quantity;
                }
                await _context.SaveChangesAsync();

                // Send cancellation email
                if (booking.User != null && booking.Event != null)
                {
                    await _emailService.SendBookingCancellationAsync(
                        booking.User.Email,
                        booking.User.FullName ?? "",
                        booking.Event.Title,
                        booking.BookingId.ToString());
                }

                TempData["Success"] = "Hủy đặt chỗ thành công";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cancelling booking: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Details", new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                    return NotFound();

                if (booking.PaymentStatus != PaymentStatus.Paid)
                {
                    TempData["Error"] = "Chỉ có thể hoàn tiền cho đặt chỗ đã thanh toán";
                    return RedirectToAction("Details", new { id });
                }

                booking.PaymentStatus = PaymentStatus.Refunded;
                if (booking.Event != null)
                {
                    booking.Event.AvailableSeats += booking.Quantity;
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Hoàn tiền thành công";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error refunding booking: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Details", new { id });
            }
        }
    }
}

