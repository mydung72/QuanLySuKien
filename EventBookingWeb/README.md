# Event Booking System - Hệ thống Quản lý Sự kiện và Đặt chỗ

## Tổng quan

Hệ thống quản lý sự kiện và đặt chỗ trực tuyến được xây dựng bằng ASP.NET MVC Core 8.0, cho phép người dùng xem, đặt chỗ và quản lý vé tham gia sự kiện, đồng thời cung cấp cho quản trị viên các công cụ quản lý sự kiện, người dùng và báo cáo thống kê.

## Công nghệ sử dụng

- **Backend**: ASP.NET MVC Core 8.0
- **Database**: SQL Server
- **Authentication**: Session-based với BCrypt password hashing
- **Email**: MailKit
- **Payment**: VNPay integration
- **QR Code**: QRCoder
- **Reports**: ClosedXML (Excel), QuestPDF (PDF)
- **Frontend**: Bootstrap 5, jQuery

## Cấu trúc dự án

### Models
- **DomainModels**: DBUser, DBEvent, DBBooking, DBTicket, DBBanner, DBNews, DBCategoryEvent
- **Enums**: UserRole, UserStatus, EventStatus, PaymentStatus
- **ViewModels**: Các ViewModels cho Account, Event, Booking, Admin, Ticket

### Services
- **IEmailService/EmailService**: Gửi email xác nhận, vé điện tử
- **IPaymentService/PaymentService**: Tích hợp cổng thanh toán VNPay
- **IQrCodeService/QrCodeService**: Tạo mã QR cho vé
- **IReportService/ReportService**: Xuất báo cáo Excel/PDF
- **IFileUploadService/FileUploadService**: Upload và quản lý file hình ảnh

### Controllers

#### User Controllers
- **HomeController**: Trang chủ với banner, sự kiện nổi bật
- **AccountController**: Đăng nhập, đăng ký, quên mật khẩu
- **EventController**: Danh sách và chi tiết sự kiện
- **BookingController**: Đặt chỗ, thanh toán, quản lý đặt chỗ
- **TicketController**: Xem và tải vé điện tử

#### Admin Controllers
- **AdminController**: Dashboard với thống kê tổng quan
- **UserManagementController**: CRUD người dùng, khóa/mở khóa tài khoản
- **EventManagementController**: CRUD sự kiện, upload poster
- **BookingManagementController**: Quản lý đặt chỗ, duyệt/hủy/hoàn tiền
- **ReportController**: Báo cáo doanh thu, người tham gia, xuất Excel
- **MediaController**: Quản lý banner và tin tức

### Helpers
- **PasswordHelper**: Hash và verify password với BCrypt
- **EmailHelper**: Template email
- **PaymentHelper**: Generate booking code, ticket code

### Attributes
- **AuthorizeUserAttribute**: Yêu cầu đăng nhập
- **AuthorizeAdminAttribute**: Yêu cầu quyền Admin

## Chức năng chính

### Dành cho User
1. **Quản lý tài khoản**
   - Đăng ký, đăng nhập
   - Quên mật khẩu, đặt lại mật khẩu

2. **Xem sự kiện**
   - Danh sách sự kiện với filter (category, date, location)
   - Chi tiết sự kiện với poster, mô tả, tin tức liên quan

3. **Đặt chỗ**
   - Đặt vé với số lượng tùy chọn
   - Thanh toán qua VNPay hoặc tiền mặt
   - Nhận email xác nhận
   - Xem lịch sử đặt chỗ
   - Hủy đặt chỗ (nếu chưa diễn ra)

4. **Quản lý vé**
   - Xem danh sách vé đã đặt
   - Xem vé điện tử với QR code
   - Tải vé PDF

### Dành cho Admin
1. **Dashboard**
   - Thống kê tổng quan: người dùng, sự kiện, đặt chỗ, doanh thu
   - Đặt chỗ gần đây
   - Sự kiện sắp diễn ra

2. **Quản lý người dùng**
   - CRUD người dùng
   - Khóa/mở khóa tài khoản
   - Filter theo role, status

3. **Quản lý sự kiện**
   - CRUD sự kiện
   - Upload poster
   - Cập nhật trạng thái (Upcoming, Ongoing, Ended)
   - Phân loại theo category

4. **Quản lý đặt chỗ**
   - Xem danh sách đặt chỗ với filter
   - Duyệt đặt chỗ
   - Hủy đặt chỗ
   - Hoàn tiền

5. **Báo cáo & Thống kê**
   - Báo cáo doanh thu theo thời gian
   - Báo cáo người tham gia theo sự kiện
   - Thống kê sự kiện phổ biến
   - Xuất Excel

6. **Quản lý nội dung**
   - Quản lý banner trang chủ
   - Quản lý tin tức
   - Upload hình ảnh

## Cấu hình

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=EventBookingDB;..."
  },
  "EmailSettings": {
    "FromEmail": "noreply@eventbooking.com",
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password"
  },
  "VNPaySettings": {
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET",
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://localhost:7012/Booking/PaymentCallback"
  }
}
```

### Database Migration
```bash
# Tạo database và tables
dotnet ef database update
```

### Tài khoản Admin mặc định
- Email: admin@gmail.com
- Password: 123456

## Cấu trúc thư mục uploads
Cần tạo các thư mục sau trong `wwwroot/uploads/`:
- events/
- banners/
- news/

## Workflow chính

### Quy trình đặt chỗ
1. User đăng nhập
2. Xem chi tiết sự kiện
3. Chọn "Đặt chỗ", nhập số lượng vé
4. Chọn phương thức thanh toán (VNPay/Cash)
5. Thanh toán thành công → Hệ thống tạo vé với QR code
6. Gửi email xác nhận với thông tin vé
7. User có thể xem và tải vé PDF

### Quy trình thanh toán VNPay
1. User chọn thanh toán VNPay
2. Redirect đến cổng VNPay
3. User thanh toán trên VNPay
4. VNPay callback về hệ thống
5. Hệ thống validate callback, cập nhật trạng thái
6. Tạo vé điện tử và gửi email

## Security Features
- Password hashing với BCrypt
- Session-based authentication
- CSRF protection với AntiForgeryToken
- Input validation với Data Annotations
- Authorization với custom attributes
- SQL injection prevention với EF Core

## API Endpoints (Controllers)

### User
- GET /Account/Login
- POST /Account/Login
- GET /Account/Register
- POST /Account/Register
- GET /Event/Index
- GET /Event/Details/{id}
- GET /Booking/Create/{eventId}
- POST /Booking/Create
- GET /Booking/MyBookings
- GET /Ticket/MyTickets
- GET /Ticket/Download/{id}

### Admin
- GET /Admin/Dashboard
- GET /UserManagement/Index
- GET /EventManagement/Index
- GET /BookingManagement/Index
- GET /Report/Revenue
- GET /Report/ExportExcel

## Notes
- Hệ thống sử dụng Session để lưu thông tin đăng nhập
- File upload giới hạn 5MB cho hình ảnh
- Email gửi qua SMTP (cần cấu hình Gmail App Password hoặc SMTP server)
- VNPay cần đăng ký tài khoản sandbox để test

## Future Enhancements
- Multi-language support
- Real-time notifications với SignalR
- Social login (Google, Facebook)
- Advanced analytics dashboard
- Mobile app
- Seat selection for events
- Discount codes và vouchers

