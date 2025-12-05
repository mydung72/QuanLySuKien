using ClosedXML.Excel;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EventBookingWeb.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IQrCodeService _qrCodeService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(ApplicationDbContext context, IQrCodeService qrCodeService, ILogger<ReportService> logger)
        {
            _context = context;
            _qrCodeService = qrCodeService;
            _logger = logger;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> ExportRevenueToExcelAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.User)
                    .Where(b => b.PaymentStatus == PaymentStatus.Paid);

                if (startDate.HasValue)
                    query = query.Where(b => b.BookingDate >= startDate.Value);
                
                if (endDate.HasValue)
                    query = query.Where(b => b.BookingDate <= endDate.Value);

                var bookings = await query.OrderBy(b => b.BookingDate).ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Doanh thu");

                // Headers
                worksheet.Cell(1, 1).Value = "Mã đặt chỗ";
                worksheet.Cell(1, 2).Value = "Sự kiện";
                worksheet.Cell(1, 3).Value = "Người đặt";
                worksheet.Cell(1, 4).Value = "Email";
                worksheet.Cell(1, 5).Value = "Số lượng";
                worksheet.Cell(1, 6).Value = "Tổng tiền";
                worksheet.Cell(1, 7).Value = "Ngày đặt";
                worksheet.Cell(1, 8).Value = "Trạng thái";

                // Data
                int row = 2;
                foreach (var booking in bookings)
                {
                    worksheet.Cell(row, 1).Value = booking.BookingId;
                    worksheet.Cell(row, 2).Value = booking.Event?.Title ?? "";
                    worksheet.Cell(row, 3).Value = booking.User?.FullName ?? "";
                    worksheet.Cell(row, 4).Value = booking.User?.Email ?? "";
                    worksheet.Cell(row, 5).Value = booking.Quantity;
                    worksheet.Cell(row, 6).Value = booking.TotalAmount;
                    worksheet.Cell(row, 7).Value = booking.BookingDate;
                    worksheet.Cell(row, 8).Value = booking.PaymentStatus.ToString();
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting revenue to Excel: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> ExportEventAttendeesToExcelAsync(int eventId)
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.User)
                    .Where(b => b.EventId == eventId && b.PaymentStatus == PaymentStatus.Paid)
                    .OrderBy(b => b.BookingDate)
                    .ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Danh sách người tham gia");

                // Headers
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Họ tên";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Số điện thoại";
                worksheet.Cell(1, 5).Value = "Số lượng vé";
                worksheet.Cell(1, 6).Value = "Ngày đặt";

                // Data
                int row = 2;
                foreach (var booking in bookings)
                {
                    worksheet.Cell(row, 1).Value = row - 1;
                    worksheet.Cell(row, 2).Value = booking.User?.FullName ?? "";
                    worksheet.Cell(row, 3).Value = booking.User?.Email ?? "";
                    worksheet.Cell(row, 4).Value = booking.User?.Phone ?? "";
                    worksheet.Cell(row, 5).Value = booking.Quantity;
                    worksheet.Cell(row, 6).Value = booking.BookingDate;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting event attendees to Excel: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> ExportBookingsToExcelAsync(DateTime? startDate, DateTime? endDate)
        {
            return await ExportRevenueToExcelAsync(startDate, endDate);
        }

        public async Task<byte[]> GenerateTicketPdfAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.User)
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.Event)
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);

                if (ticket == null)
                    throw new Exception("Ticket not found");

                var qrCodeBytes = _qrCodeService.GenerateQRCodeBytes(ticket.QRCodeData);

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text("VÉ ĐIỆN TỬ")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(20);

                                column.Item().Text($"Sự kiện: {ticket.Booking?.Event?.Title ?? ""}").FontSize(16).SemiBold();
                                column.Item().Text($"Mã vé: {ticket.TicketCode}").FontSize(14);
                                column.Item().Text($"Người tham dự: {ticket.User?.FullName ?? ""}");
                                column.Item().Text($"Email: {ticket.User?.Email ?? ""}");
                                column.Item().Text($"Ngày sự kiện: {ticket.Booking?.Event?.StartDate:dd/MM/yyyy HH:mm}");
                                column.Item().Text($"Địa điểm: {ticket.Booking?.Event?.Location ?? ""}");

                                column.Item().PaddingTop(20).Image(qrCodeBytes).FitWidth();

                                column.Item().PaddingTop(20).Text("Vui lòng xuất trình mã QR này tại cổng vào sự kiện.")
                                    .FontSize(10).Italic();
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Trang ");
                                x.CurrentPageNumber();
                            });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating ticket PDF: {ex.Message}");
                throw;
            }
        }
    }
}

