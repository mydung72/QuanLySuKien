using EventBookingWeb.Attributes;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Services;
using EventBookingWeb.ViewModels.Ticket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers
{
    [AuthorizeUser]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IQrCodeService _qrCodeService;
        private readonly IReportService _reportService;
        private readonly ILogger<TicketController> _logger;

        public TicketController(
            ApplicationDbContext context,
            IQrCodeService qrCodeService,
            IReportService reportService,
            ILogger<TicketController> logger)
        {
            _context = context;
            _qrCodeService = qrCodeService;
            _reportService = reportService;
            _logger = logger;
        }

        // DEBUG: Test action để kiểm tra tickets trong DB
        public async Task<IActionResult> Debug()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var now = DateTime.Now;
            
            var allTickets = await _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b!.Event)
                .Where(t => t.UserID == userId)
                .ToListAsync();
            
            var upcomingTickets = allTickets.Where(t => t.Booking?.Event?.StartDate >= now).ToList();
            
            var debug = new
            {
                UserId = userId,
                CurrentTime = now,
                TotalTickets = allTickets.Count,
                UpcomingTickets = upcomingTickets.Count,
                AllTicketsDetail = allTickets.Select(t => new {
                    t.TicketId,
                    t.TicketCode,
                    t.BookingID,
                    EventTitle = t.Booking?.Event?.Title ?? "NULL",
                    EventStartDate = t.Booking?.Event?.StartDate.ToString("yyyy-MM-dd HH:mm") ?? "NULL",
                    IsUpcoming = t.Booking?.Event?.StartDate >= now,
                    BookingStatus = t.Booking?.PaymentStatus.ToString() ?? "NULL"
                }).ToList()
            };
            
            return Json(debug);
        }

        // DEBUG: Tạo test data
        public async Task<IActionResult> CreateTestData()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                if (userId == 0)
                    return Json(new { error = "User not logged in" });

                // Lấy event sắp tới đầu tiên
                var upcomingEvent = await _context.Events
                    .Where(e => e.StartDate >= DateTime.Now && e.AvailableSeats > 0)
                    .OrderBy(e => e.StartDate)
                    .FirstOrDefaultAsync();

                if (upcomingEvent == null)
                    return Json(new { error = "No upcoming events found" });

                // Tạo booking
                var booking = new DBBooking
                {
                    BookingId = 0,
                    EventId = upcomingEvent.EventId,
                    UserId = userId,
                    Quantity = 2,
                    TotalAmount = (Decimal)(upcomingEvent.TicketPrice * 2),
                    PaymentStatus = EventBookingWeb.Models.Enums.PaymentStatus.Reserved,
                    PaymentMethod = "Test - Thanh toán tại sự kiện",
                    BookingDate = DateTime.Now
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Tạo tickets
                var ticketsCreated = new List<string>();
                for (int i = 0; i < booking.Quantity; i++)
                {
                    var ticketCode = $"TEST{DateTime.Now:yyyyMMddHHmmss}{booking.BookingId:D6}{i + 1:D2}";
                    var ticket = new DBTicket
                    {
                        TicketId = 0,
                        BookingID = booking.BookingId,
                        UserID = userId,
                        TicketCode = ticketCode,
                        QRCodeData = ticketCode,
                        CreatedAt = DateTime.Now
                    };
                    _context.Tickets.Add(ticket);
                    ticketsCreated.Add(ticketCode);
                }
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true,
                    message = "Test data created",
                    bookingId = booking.BookingId,
                    eventTitle = upcomingEvent.Title,
                    ticketsCreated = ticketsCreated
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        public async Task<IActionResult> MyTickets(int page = 1)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                if (userId == 0)
                    return RedirectToAction("Login", "Account");

                int pageSize = 12;
                var now = DateTime.Now;
                
                // Query từ Tickets để đảm bảo chỉ lấy vé thực sự tồn tại
                var query = _context.Tickets
                    .Include(t => t.Booking)
                        .ThenInclude(b => b!.Event)
                    .Where(t => t.UserID == userId 
                        && t.Booking!.Event!.StartDate >= now)
                    .OrderBy(t => t.Booking!.Event!.StartDate);

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var tickets = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TicketItemViewModel
                    {
                        TicketId = t.TicketId,
                        TicketCode = t.TicketCode,
                        EventTitle = t.Booking!.Event!.Title,
                        EventImageUrl = t.Booking.Event.ImageUrl,
                        EventStartDate = t.Booking.Event.StartDate,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                var viewModel = new TicketListViewModel
                {
                    Tickets = tickets,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading my tickets: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách vé.";
                return RedirectToAction("Index", "Home");
            }
        }
       
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                
                var ticket = await _context.Tickets
                    .Include(t => t.User)
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.Event)
                    .FirstOrDefaultAsync(t => t.TicketId == id && t.UserID == userId);

                if (ticket == null)
                    return NotFound();

                var qrCodeBase64 = _qrCodeService.GenerateQRCode(ticket.QRCodeData);

                var viewModel = new TicketViewModel
                {
                    TicketId = ticket.TicketId,
                    TicketCode = ticket.TicketCode,
                    QRCodeData = ticket.QRCodeData,
                    QRCodeBase64 = qrCodeBase64,
                    CreatedAt = ticket.CreatedAt,
                    Event = ticket.Booking?.Event!,
                    Booking = ticket.Booking!,
                    User = ticket.User!
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading ticket details: {ex.Message}");
                return NotFound();
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketId == id && t.UserID == userId);

                if (ticket == null)
                    return NotFound();

                var pdfBytes = await _reportService.GenerateTicketPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"Ticket_{ticket.TicketCode}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading ticket: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi tải vé. Vui lòng thử lại.";
                return RedirectToAction("Details", new { id });
            }
        }
    }
}

