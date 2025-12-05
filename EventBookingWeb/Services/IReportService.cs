namespace EventBookingWeb.Services
{
    public interface IReportService
    {
        Task<byte[]> ExportRevenueToExcelAsync(DateTime? startDate, DateTime? endDate);
        Task<byte[]> ExportEventAttendeesToExcelAsync(int eventId);
        Task<byte[]> ExportBookingsToExcelAsync(DateTime? startDate, DateTime? endDate);
        Task<byte[]> GenerateTicketPdfAsync(int ticketId);
    }
}

