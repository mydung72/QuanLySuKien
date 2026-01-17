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

