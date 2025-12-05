namespace EventBookingWeb.Services
{
    public interface IQrCodeService
    {
        string GenerateQRCode(string data);
        byte[] GenerateQRCodeBytes(string data);
    }
}

