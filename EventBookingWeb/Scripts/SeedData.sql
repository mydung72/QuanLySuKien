-- =============================================
-- Script SQL để seed dữ liệu mẫu cho Event Booking System
-- Database: SQL Server
-- =============================================

USE EventBookingDB;
GO

-- Xóa dữ liệu cũ (nếu cần)
-- DELETE FROM Carts;
-- DELETE FROM Tickets;
-- DELETE FROM Bookings;
-- DELETE FROM News;
-- DELETE FROM DBEventCategoryEvent;
-- DELETE FROM Events;
-- DELETE FROM CategoryEvents;
-- DELETE FROM Banners;
-- DELETE FROM Users WHERE UserId > 1; -- Giữ lại admin mặc định
-- GO

-- =============================================
-- 1. CategoryEvents (10 danh mục sự kiện)
-- =============================================
SET IDENTITY_INSERT CategoryEvents ON;
GO

INSERT INTO CategoryEvents (CategoryEventId, CategoryEventName, CategoryEventDescription) VALUES
(1, N'Hội thảo', N'Các sự kiện hội thảo, seminar về công nghệ, kinh doanh'),
(2, N'Âm nhạc', N'Các buổi hòa nhạc, concert, live show'),
(3, N'Thể thao', N'Các sự kiện thể thao, giải đấu, marathon'),
(4, N'Giáo dục', N'Workshop, khóa học, training'),
(5, N'Văn hóa', N'Triển lãm, festival văn hóa'),
(6, N'Kinh doanh', N'Networking, startup event, business forum'),
(7, N'Giải trí', N'Comedy show, game show, entertainment'),
(8, N'Ẩm thực', N'Food festival, cooking class'),
(9, N'Nghệ thuật', N'Art exhibition, theater, dance'),
(10, N'Cộng đồng', N'Community event, charity, volunteer');

SET IDENTITY_INSERT CategoryEvents OFF;
GO

-- =============================================
-- 2. Users (15 người dùng: 2 Admin + 13 User)
-- =============================================
SET IDENTITY_INSERT Users ON;
GO

INSERT INTO Users (UserId, FullName, Email, PasswordHash, Phone, Role, UserStatus, CreatedAt) VALUES
-- Admin accounts
(2, N'Nguyễn Văn Admin', 'admin2@eventbooking.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0901234568', 1, 0, '2024-01-15 10:00:00'),

-- User accounts
(3, N'Trần Thị Hoa', 'hoa.tran@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0912345678', 0, 0, '2024-02-01 08:30:00'),
(4, N'Lê Văn Nam', 'nam.le@yahoo.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0923456789', 0, 0, '2024-02-05 14:20:00'),
(5, N'Phạm Thị Lan', 'lan.pham@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0934567890', 0, 0, '2024-02-10 09:15:00'),
(6, N'Hoàng Văn Đức', 'duc.hoang@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0945678901', 0, 0, '2024-02-15 16:45:00'),
(7, N'Vũ Thị Mai', 'mai.vu@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0956789012', 0, 0, '2024-02-20 11:30:00'),
(8, N'Đỗ Văn Tuấn', 'tuan.do@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0967890123', 0, 0, '2024-02-25 13:20:00'),
(9, N'Bùi Thị Hương', 'huong.bui@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0978901234', 0, 0, '2024-03-01 10:00:00'),
(10, N'Ngô Văn Hùng', 'hung.ngo@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0989012345', 0, 0, '2024-03-05 15:30:00'),
(11, N'Dương Thị Linh', 'linh.duong@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0990123456', 0, 0, '2024-03-10 09:45:00'),
(12, N'Đinh Văn Quang', 'quang.dinh@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0901234569', 0, 0, '2024-03-15 12:15:00'),
(13, N'Lý Thị Thảo', 'thao.ly@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0912345670', 0, 0, '2024-03-20 14:00:00'),
(14, N'Trịnh Văn Minh', 'minh.trinh@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0923456780', 0, 0, '2024-03-25 16:30:00'),
(15, N'Võ Thị Nga', 'nga.vo@gmail.com', '$2a$11$K7L1OJ45/4O2NSj9YyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJYZeYyJY', '0934567891', 0, 1, '2024-03-30 10:20:00'); -- Locked user

SET IDENTITY_INSERT Users OFF;
GO

-- =============================================
-- 3. Events (15 sự kiện)
-- =============================================
SET IDENTITY_INSERT Events ON;
GO

INSERT INTO Events (EventId, EventCategoryId, Title, EventDescription, EventContentHtml, Location, StartDate, EndDate, TotalSeats, AvailableSeats, TicketPrice, ImageUrl, EventStatus, CreatedAt) VALUES
(1, N'[1,4]', N'Hội thảo Công nghệ AI 2024', N'Hội thảo về trí tuệ nhân tạo và ứng dụng trong thực tế', N'<h2>Giới thiệu</h2><p>Hội thảo về công nghệ AI với các chuyên gia hàng đầu...</p>', N'Trung tâm Hội nghị Quốc gia, Hà Nội', '2024-12-15 09:00:00', '2024-12-15 17:00:00', 500, 350, 500000, '/uploads/events/ai-conference.jpg', 0, '2024-11-01 10:00:00'),
(2, N'[2,7]', N'Concert Nhạc Pop Việt Nam', N'Đêm nhạc với các ca sĩ hàng đầu Việt Nam', N'<h2>Chương trình</h2><p>Buổi biểu diễn với nhiều ca sĩ nổi tiếng...</p>', N'Sân vận động Quốc gia Mỹ Đình, Hà Nội', '2024-12-20 19:00:00', '2024-12-20 22:00:00', 10000, 2500, 800000, '/uploads/events/pop-concert.jpg', 0, '2024-11-05 14:00:00'),
(3, N'[3,10]', N'Marathon Hà Nội 2024', N'Giải chạy marathon quốc tế tại Hà Nội', N'<h2>Thông tin</h2><p>Giải chạy với các cự ly 5km, 10km, 21km, 42km...</p>', N'Vườn hoa Lý Thái Tổ, Hà Nội', '2024-12-10 06:00:00', '2024-12-10 12:00:00', 5000, 1200, 300000, '/uploads/events/marathon.jpg', 0, '2024-11-10 08:00:00'),
(4, N'[4,6]', N'Workshop Digital Marketing', N'Khóa học về marketing số cho doanh nghiệp', N'<h2>Nội dung</h2><p>Học về SEO, SEM, Social Media Marketing...</p>', N'Trung tâm Đào tạo ABC, TP.HCM', '2024-12-18 09:00:00', '2024-12-18 17:00:00', 100, 45, 1500000, '/uploads/events/digital-marketing.jpg', 0, '2024-11-12 10:00:00'),
(5, N'[8,5]', N'Festival Ẩm thực Việt Nam', N'Lễ hội ẩm thực với các món ăn đặc sắc', N'<h2>Chương trình</h2><p>Hơn 100 gian hàng ẩm thực từ khắp cả nước...</p>', N'Công viên Thống Nhất, Hà Nội', '2024-12-25 10:00:00', '2024-12-27 22:00:00', 2000, 800, 200000, '/uploads/events/food-festival.jpg', 0, '2024-11-15 11:00:00'),
(6, N'[9,5]', N'Triển lãm Nghệ thuật Đương đại', N'Triển lãm các tác phẩm nghệ thuật đương đại', N'<h2>Giới thiệu</h2><p>Hơn 50 tác phẩm từ các nghệ sĩ trong nước và quốc tế...</p>', N'Bảo tàng Mỹ thuật Việt Nam, Hà Nội', '2024-12-05 09:00:00', '2024-12-30 17:00:00', 300, 150, 100000, '/uploads/events/art-exhibition.jpg', 1, '2024-11-01 09:00:00'),
(7, N'[6,10]', N'Startup Networking Event', N'Sự kiện kết nối các startup và nhà đầu tư', N'<h2>Chương trình</h2><p>Pitch session, networking, workshop...</p>', N'Co-working Space XYZ, TP.HCM', '2024-12-22 14:00:00', '2024-12-22 18:00:00', 200, 80, 500000, '/uploads/events/startup-networking.jpg', 0, '2024-11-18 13:00:00'),
(8, N'[7]', N'Comedy Show Hài kịch', N'Đêm hài kịch với các nghệ sĩ nổi tiếng', N'<h2>Diễn viên</h2><p>Xuân Bắc, Trấn Thành, Trường Giang...</p>', N'Nhà hát Lớn Hà Nội', '2024-12-28 20:00:00', '2024-12-28 22:30:00', 800, 200, 600000, '/uploads/events/comedy-show.jpg', 0, '2024-11-20 15:00:00'),
(9, N'[4]', N'Khóa học Lập trình Web', N'Khóa học lập trình web full-stack', N'<h2>Nội dung</h2><p>HTML, CSS, JavaScript, React, Node.js...</p>', N'Trung tâm Đào tạo IT, Đà Nẵng', '2024-12-12 08:00:00', '2024-12-16 17:00:00', 50, 15, 5000000, '/uploads/events/web-programming.jpg', 0, '2024-11-08 10:00:00'),
(10, N'[5,10]', N'Lễ hội Văn hóa Dân gian', N'Festival văn hóa các dân tộc Việt Nam', N'<h2>Hoạt động</h2><p>Biểu diễn múa, hát, trò chơi dân gian...</p>', N'Làng Văn hóa Du lịch, Hà Nội', '2024-12-30 08:00:00', '2025-01-02 22:00:00', 5000, 2000, 150000, '/uploads/events/cultural-festival.jpg', 0, '2024-11-22 12:00:00'),
(11, N'[3,10]', N'Giải đấu Bóng đá Cộng đồng', N'Giải bóng đá giao hữu các đội cộng đồng', N'<h2>Thể lệ</h2><p>Giải đấu 16 đội, loại trực tiếp...</p>', N'Sân vận động Thể thao, TP.HCM', '2024-12-08 08:00:00', '2024-12-08 18:00:00', 2000, 500, 100000, '/uploads/events/football-tournament.jpg', 0, '2024-11-05 09:00:00'),
(12, N'[1,6]', N'Seminar Blockchain & Crypto', N'Hội thảo về công nghệ blockchain và tiền điện tử', N'<h2>Chủ đề</h2><p>Blockchain, DeFi, NFT, Web3...</p>', N'Khách sạn Grand, Hà Nội', '2024-12-14 09:00:00', '2024-12-14 17:00:00', 300, 120, 800000, '/uploads/events/blockchain-seminar.jpg', 0, '2024-11-10 11:00:00'),
(13, N'[4,10]', N'Yoga & Wellness Retreat', N'Khóa học yoga và chăm sóc sức khỏe', N'<h2>Chương trình</h2><p>Yoga, thiền, massage, healthy food...</p>', N'Resort Sapa, Lào Cai', '2024-12-24 07:00:00', '2024-12-26 18:00:00', 40, 10, 3000000, '/uploads/events/yoga-retreat.jpg', 0, '2024-11-15 14:00:00'),
(14, N'[2]', N'Concert Nhạc Jazz Quốc tế', N'Đêm nhạc jazz với các nghệ sĩ quốc tế', N'<h2>Nghệ sĩ</h2><p>Herbie Hancock, Pat Metheny...</p>', N'Opera House, TP.HCM', '2024-12-19 19:30:00', '2024-12-19 22:00:00', 1200, 300, 1200000, '/uploads/events/jazz-concert.jpg', 0, '2024-11-12 16:00:00'),
(15, N'[1,6]', N'Tech Conference 2024', N'Hội nghị công nghệ lớn nhất năm', N'<h2>Diễn giả</h2><p>Các CEO và CTO từ các công ty công nghệ hàng đầu...</p>', N'Trung tâm Hội nghị, TP.HCM', '2024-11-30 09:00:00', '2024-11-30 18:00:00', 1000, 0, 2000000, '/uploads/events/tech-conference.jpg', 2, '2024-10-15 10:00:00'); -- Ended event

SET IDENTITY_INSERT Events OFF;
GO

-- =============================================
-- 4. DBEventCategoryEvent (Bảng trung gian - gán category cho events)
-- =============================================
INSERT INTO DBEventCategoryEvent (EventId, CategoryEventId) VALUES
(1, 1), (1, 4), -- AI Conference: Hội thảo, Giáo dục
(2, 2), (2, 7), -- Pop Concert: Âm nhạc, Giải trí
(3, 3), (3, 10), -- Marathon: Thể thao, Cộng đồng
(4, 4), (4, 6), -- Digital Marketing: Giáo dục, Kinh doanh
(5, 8), (5, 5), -- Food Festival: Ẩm thực, Văn hóa
(6, 9), (6, 5), -- Art Exhibition: Nghệ thuật, Văn hóa
(7, 6), (7, 10), -- Startup Networking: Kinh doanh, Cộng đồng
(8, 7), -- Comedy Show: Giải trí
(9, 4), -- Web Programming: Giáo dục
(10, 5), (10, 10), -- Cultural Festival: Văn hóa, Cộng đồng
(11, 3), (11, 10), -- Football Tournament: Thể thao, Cộng đồng
(12, 1), (12, 6), -- Blockchain Seminar: Hội thảo, Kinh doanh
(13, 4), (13, 10), -- Yoga Retreat: Giáo dục, Cộng đồng
(14, 2), -- Jazz Concert: Âm nhạc
(15, 1), (15, 6); -- Tech Conference: Hội thảo, Kinh doanh
GO

-- =============================================
-- 5. Carts (15 giỏ hàng)
-- =============================================
SET IDENTITY_INSERT Carts ON;
GO

INSERT INTO Carts (CartId, UserId, EventId, Quantity, CreatedAt, UpdatedAt) VALUES
(1, 3, 1, 2, '2024-11-25 10:00:00', '2024-11-25 10:00:00'),
(2, 3, 2, 1, '2024-11-26 14:00:00', '2024-11-26 14:00:00'),
(3, 4, 3, 3, '2024-11-27 09:00:00', '2024-11-27 09:00:00'),
(4, 4, 4, 1, '2024-11-28 15:00:00', '2024-11-28 15:00:00'),
(5, 5, 5, 2, '2024-11-29 11:00:00', '2024-11-29 11:00:00'),
(6, 5, 6, 1, '2024-11-30 13:00:00', '2024-11-30 13:00:00'),
(7, 6, 7, 1, '2024-12-01 10:00:00', '2024-12-01 10:00:00'),
(8, 6, 8, 2, '2024-12-02 16:00:00', '2024-12-02 16:00:00'),
(9, 7, 9, 1, '2024-12-03 09:00:00', '2024-12-03 09:00:00'),
(10, 7, 10, 4, '2024-12-04 14:00:00', '2024-12-04 14:00:00'),
(11, 8, 11, 2, '2024-12-05 11:00:00', '2024-12-05 11:00:00'),
(12, 8, 12, 1, '2024-12-06 15:00:00', '2024-12-06 15:00:00'),
(13, 9, 13, 1, '2024-12-07 10:00:00', '2024-12-07 10:00:00'),
(14, 9, 14, 2, '2024-12-08 12:00:00', '2024-12-08 12:00:00'),
(15, 10, 1, 1, '2024-12-09 13:00:00', '2024-12-09 13:00:00');
GO

SET IDENTITY_INSERT Carts OFF;
GO

-- =============================================
-- 6. Bookings (20 đặt chỗ)
-- =============================================
SET IDENTITY_INSERT Bookings ON;
GO

INSERT INTO Bookings (BookingId, EventId, UserId, Quantity, TotalAmount, PaymentStatus, PaymentMethod, BookingDate) VALUES
(1, 1, 3, 2, 1000000, 2, N'VNPay', '2024-11-05 10:30:00'),
(2, 2, 4, 1, 800000, 2, N'VNPay', '2024-11-06 14:20:00'),
(3, 3, 5, 3, 900000, 2, N'Cash', '2024-11-07 09:15:00'),
(4, 4, 6, 1, 1500000, 2, N'VNPay', '2024-11-08 16:45:00'),
(5, 5, 7, 2, 400000, 0, N'Pending', '2024-11-09 11:30:00'),
(6, 6, 8, 1, 100000, 2, N'VNPay', '2024-11-10 13:20:00'),
(7, 7, 9, 1, 500000, 2, N'VNPay', '2024-11-11 10:00:00'),
(8, 8, 10, 2, 1200000, 1, N'Cash', '2024-11-12 15:30:00'),
(9, 9, 11, 1, 5000000, 2, N'VNPay', '2024-11-13 09:45:00'),
(10, 10, 12, 4, 600000, 0, N'Pending', '2024-11-14 12:15:00'),
(11, 11, 13, 2, 200000, 2, N'VNPay', '2024-11-15 14:00:00'),
(12, 12, 3, 1, 800000, 2, N'VNPay', '2024-11-16 16:30:00'),
(13, 13, 4, 1, 3000000, 3, N'VNPay', '2024-11-17 10:20:00'), -- Cancelled
(14, 14, 5, 2, 2400000, 2, N'VNPay', '2024-11-18 11:00:00'),
(15, 1, 6, 1, 500000, 2, N'VNPay', '2024-11-19 15:00:00'),
(16, 2, 7, 3, 2400000, 2, N'VNPay', '2024-11-20 09:30:00'),
(17, 3, 8, 2, 600000, 2, N'Cash', '2024-11-21 13:45:00'),
(18, 4, 9, 1, 1500000, 0, N'Pending', '2024-11-22 10:15:00'),
(19, 5, 10, 2, 400000, 2, N'VNPay', '2024-11-23 14:30:00'),
(20, 6, 11, 1, 100000, 2, N'VNPay', '2024-11-24 16:00:00');
GO

SET IDENTITY_INSERT Bookings OFF;
GO

-- =============================================
-- 7. Tickets (30 vé - nhiều vé cho các booking đã thanh toán)
-- =============================================
SET IDENTITY_INSERT Tickets ON;
GO

INSERT INTO Tickets (TicketId, BookingID, UserID, TicketCode, QRCodeData, CreatedAt) VALUES
-- Tickets for Booking 1 (2 tickets)
(1, 1, 3, 'TK202411051030001', 'BK1-TK1-20241105', '2024-11-05 10:35:00'),
(2, 1, 3, 'TK202411051030002', 'BK1-TK2-20241105', '2024-11-05 10:35:00'),

-- Tickets for Booking 2 (1 ticket)
(3, 2, 4, 'TK202411061420001', 'BK2-TK1-20241106', '2024-11-06 14:25:00'),

-- Tickets for Booking 3 (3 tickets)
(4, 3, 5, 'TK202411070915001', 'BK3-TK1-20241107', '2024-11-07 09:20:00'),
(5, 3, 5, 'TK202411070915002', 'BK3-TK2-20241107', '2024-11-07 09:20:00'),
(6, 3, 5, 'TK202411070915003', 'BK3-TK3-20241107', '2024-11-07 09:20:00'),

-- Tickets for Booking 4 (1 ticket)
(7, 4, 6, 'TK202411081645001', 'BK4-TK1-20241108', '2024-11-08 16:50:00'),

-- Tickets for Booking 6 (1 ticket)
(8, 6, 8, 'TK202411101320001', 'BK6-TK1-20241110', '2024-11-10 13:25:00'),

-- Tickets for Booking 7 (1 ticket)
(9, 7, 9, 'TK202411111000001', 'BK7-TK1-20241111', '2024-11-11 10:05:00'),

-- Tickets for Booking 9 (1 ticket)
(10, 9, 11, 'TK202411130945001', 'BK9-TK1-20241113', '2024-11-13 09:50:00'),

-- Tickets for Booking 11 (2 tickets)
(11, 11, 13, 'TK202411151400001', 'BK11-TK1-20241115', '2024-11-15 14:05:00'),
(12, 11, 13, 'TK202411151400002', 'BK11-TK2-20241115', '2024-11-15 14:05:00'),

-- Tickets for Booking 12 (1 ticket)
(13, 12, 3, 'TK202411161630001', 'BK12-TK1-20241116', '2024-11-16 16:35:00'),

-- Tickets for Booking 14 (2 tickets)
(14, 14, 5, 'TK202411181100001', 'BK14-TK1-20241118', '2024-11-18 11:05:00'),
(15, 14, 5, 'TK202411181100002', 'BK14-TK2-20241118', '2024-11-18 11:05:00'),

-- Tickets for Booking 15 (1 ticket)
(16, 15, 6, 'TK202411191500001', 'BK15-TK1-20241119', '2024-11-19 15:05:00'),

-- Tickets for Booking 16 (3 tickets)
(17, 16, 7, 'TK202411200930001', 'BK16-TK1-20241120', '2024-11-20 09:35:00'),
(18, 16, 7, 'TK202411200930002', 'BK16-TK2-20241120', '2024-11-20 09:35:00'),
(19, 16, 7, 'TK202411200930003', 'BK16-TK3-20241120', '2024-11-20 09:35:00'),

-- Tickets for Booking 17 (2 tickets)
(20, 17, 8, 'TK202411211345001', 'BK17-TK1-20241121', '2024-11-21 13:50:00'),
(21, 17, 8, 'TK202411211345002', 'BK17-TK2-20241121', '2024-11-21 13:50:00'),

-- Tickets for Booking 19 (2 tickets)
(22, 19, 10, 'TK202411231430001', 'BK19-TK1-20241123', '2024-11-23 14:35:00'),
(23, 19, 10, 'TK202411231430002', 'BK19-TK2-20241123', '2024-11-23 14:35:00'),

-- Tickets for Booking 20 (1 ticket)
(24, 20, 11, 'TK202411241600001', 'BK20-TK1-20241124', '2024-11-24 16:05:00'),

-- Additional tickets for variety
(25, 1, 3, 'TK202411051030003', 'BK1-TK3-20241105', '2024-11-05 10:35:00'),
(26, 2, 4, 'TK202411061420002', 'BK2-TK2-20241106', '2024-11-06 14:25:00'),
(27, 3, 5, 'TK202411070915004', 'BK3-TK4-20241107', '2024-11-07 09:20:00'),
(28, 4, 6, 'TK202411081645002', 'BK4-TK2-20241108', '2024-11-08 16:50:00'),
(29, 6, 8, 'TK202411101320002', 'BK6-TK2-20241110', '2024-11-10 13:25:00'),
(30, 7, 9, 'TK202411111000002', 'BK7-TK2-20241111', '2024-11-11 10:05:00');
GO

SET IDENTITY_INSERT Tickets OFF;
GO

-- =============================================
-- 8. Banners (10 banner)
-- =============================================
SET IDENTITY_INSERT Banners ON;
GO

INSERT INTO Banners (BannerID, ImageUrl, LinkUrl, IsActive, CreatedAt) VALUES
(1, '/uploads/banners/banner1.jpg', '/Event/Details/1', 1, '2024-11-01 10:00:00'),
(2, '/uploads/banners/banner2.jpg', '/Event/Details/2', 1, '2024-11-02 11:00:00'),
(3, '/uploads/banners/banner3.jpg', '/Event/Details/3', 1, '2024-11-03 12:00:00'),
(4, '/uploads/banners/banner4.jpg', '/Event/Details/4', 1, '2024-11-04 13:00:00'),
(5, '/uploads/banners/banner5.jpg', '/Event/Details/5', 1, '2024-11-05 14:00:00'),
(6, '/uploads/banners/banner6.jpg', '/Event/Index', 1, '2024-11-06 15:00:00'),
(7, '/uploads/banners/banner7.jpg', '/Event/Details/7', 0, '2024-11-07 16:00:00'), -- Inactive
(8, '/uploads/banners/banner8.jpg', '/Event/Details/8', 1, '2024-11-08 17:00:00'),
(9, '/uploads/banners/banner9.jpg', '/Event/Details/9', 1, '2024-11-09 18:00:00'),
(10, '/uploads/banners/banner10.jpg', '/Event/Index', 1, '2024-11-10 19:00:00');
GO

SET IDENTITY_INSERT Banners OFF;
GO

-- =============================================
-- 9. News (15 tin tức)
-- =============================================
SET IDENTITY_INSERT News ON;
GO

INSERT INTO News (NewsId, EventId, Title, NewsContentHtml, ImageUrl, IsVisible, CreatedAt) VALUES
(1, 1, N'AI Conference 2024: Cơ hội vàng cho các nhà phát triển', N'<h2>Tin tức</h2><p>Hội thảo AI sẽ quy tụ hơn 50 chuyên gia hàng đầu...</p>', '/uploads/news/news1.jpg', 1, '2024-11-01 10:00:00'),
(2, 2, N'Concert Pop Việt Nam: Danh sách nghệ sĩ tham gia', N'<h2>Thông tin</h2><p>Đêm nhạc sẽ có sự góp mặt của nhiều ca sĩ nổi tiếng...</p>', '/uploads/news/news2.jpg', 1, '2024-11-02 11:00:00'),
(3, 3, N'Marathon Hà Nội 2024: Đăng ký đã mở', N'<h2>Thông báo</h2><p>Giải chạy marathon lớn nhất năm đã bắt đầu nhận đăng ký...</p>', '/uploads/news/news3.jpg', 1, '2024-11-03 12:00:00'),
(4, 4, N'Workshop Digital Marketing: Kiến thức thực tế', N'<h2>Giới thiệu</h2><p>Khóa học sẽ cung cấp kiến thức marketing số...</p>', '/uploads/news/news4.jpg', 1, '2024-11-04 13:00:00'),
(5, 5, N'Festival Ẩm thực: Hơn 100 gian hàng tham gia', N'<h2>Tin tức</h2><p>Lễ hội ẩm thực sẽ có hơn 100 gian hàng...</p>', '/uploads/news/news5.jpg', 1, '2024-11-05 14:00:00'),
(6, 6, N'Triển lãm Nghệ thuật: Khai mạc thành công', N'<h2>Báo cáo</h2><p>Triển lãm đã khai mạc với sự tham gia đông đảo...</p>', '/uploads/news/news6.jpg', 1, '2024-11-06 15:00:00'),
(7, 7, N'Startup Networking: Cơ hội kết nối', N'<h2>Thông tin</h2><p>Sự kiện sẽ giúp các startup kết nối với nhà đầu tư...</p>', '/uploads/news/news7.jpg', 1, '2024-11-07 16:00:00'),
(8, 8, N'Comedy Show: Vé đang được bán chạy', N'<h2>Tin tức</h2><p>Vé cho đêm hài kịch đang được bán rất chạy...</p>', '/uploads/news/news8.jpg', 1, '2024-11-08 17:00:00'),
(9, 9, N'Khóa học Lập trình: Chỉ còn 15 chỗ', N'<h2>Thông báo</h2><p>Khóa học lập trình web chỉ còn 15 chỗ trống...</p>', '/uploads/news/news9.jpg', 1, '2024-11-09 18:00:00'),
(10, 10, N'Lễ hội Văn hóa: Chương trình đặc sắc', N'<h2>Giới thiệu</h2><p>Lễ hội sẽ có nhiều hoạt động văn hóa đặc sắc...</p>', '/uploads/news/news10.jpg', 1, '2024-11-10 19:00:00'),
(11, 11, N'Giải đấu Bóng đá: Vòng loại đã kết thúc', N'<h2>Kết quả</h2><p>Vòng loại giải đấu bóng đá đã kết thúc...</p>', '/uploads/news/news11.jpg', 1, '2024-11-11 20:00:00'),
(12, 12, N'Seminar Blockchain: Diễn giả quốc tế', N'<h2>Thông tin</h2><p>Hội thảo sẽ có các diễn giả từ nước ngoài...</p>', '/uploads/news/news12.jpg', 1, '2024-11-12 21:00:00'),
(13, 13, N'Yoga Retreat: Trải nghiệm thư giãn', N'<h2>Giới thiệu</h2><p>Khóa học yoga sẽ mang đến trải nghiệm thư giãn...</p>', '/uploads/news/news13.jpg', 1, '2024-11-13 22:00:00'),
(14, 14, N'Jazz Concert: Đêm nhạc đặc biệt', N'<h2>Tin tức</h2><p>Đêm nhạc jazz sẽ có các nghệ sĩ quốc tế...</p>', '/uploads/news/news14.jpg', 1, '2024-11-14 23:00:00'),
(15, 15, N'Tech Conference 2024: Đã kết thúc thành công', N'<h2>Báo cáo</h2><p>Hội nghị công nghệ đã kết thúc với nhiều thành công...</p>', '/uploads/news/news15.jpg', 1, '2024-12-01 10:00:00');
GO

SET IDENTITY_INSERT News OFF;
GO

-- =============================================
-- Hoàn thành
-- =============================================
PRINT 'Đã import thành công dữ liệu mẫu!';
PRINT '- 10 Categories';
PRINT '- 15 Users (2 Admin + 13 User)';
PRINT '- 15 Events';
PRINT '- 15 Carts';
PRINT '- 20 Bookings';
PRINT '- 30 Tickets';
PRINT '- 10 Banners';
PRINT '- 15 News';
GO

