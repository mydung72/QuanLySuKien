using EventBookingWeb.Attributes;
using EventBookingWeb.Helpers;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using EventBookingWeb.Services;
using EventBookingWeb.ViewModels.Booking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers
{
    [AuthorizeUser]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly IQrCodeService _qrCodeService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            ApplicationDbContext context,
            IEmailService emailService,
            IPaymentService paymentService,
            IQrCodeService qrCodeService,
            ILogger<BookingController> logger)
        {
            _context = context;
            _emailService = emailService;
            _paymentService = paymentService;
            _qrCodeService = qrCodeService;
            _logger = logger;
        }

        public async Task<IActionResult> Create(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
                return NotFound();

            var viewModel = new BookingCreateViewModel
            {
                EventId = eventId,
                EventTitle = eventItem.Title,
                TicketPrice = eventItem.TicketPrice,
                AvailableSeats = eventItem.AvailableSeats,
                Quantity = 1
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var eventItem = await _context.Events.FindAsync(model.EventId);
                    model.EventTitle = eventItem?.Title;
                    model.TicketPrice = eventItem?.TicketPrice ?? 0;
                    model.AvailableSeats = eventItem?.AvailableSeats ?? 0;
                    return View(model);
                }

                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                var eventToBook = await _context.Events.FindAsync(model.EventId);

                if (eventToBook == null)
                    return NotFound();

                if (eventToBook.AvailableSeats < model.Quantity)
                {
                    ModelState.AddModelError("", "Không đủ chỗ trống");
                    return View(model);
                }

                var booking = new DBBooking
                {
                    BookingId = 0,
                    EventId = model.EventId,
                    UserId = userId,
                    Quantity = model.Quantity,
                    TotalAmount = (decimal)(model.Quantity * eventToBook.TicketPrice),
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentMethod = "Pending",
                    BookingDate = DateTime.Now
                };

                _context.Bookings.Add(booking);
                eventToBook.AvailableSeats -= model.Quantity;
                await _context.SaveChangesAsync();

                // Send booking confirmation email
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    await _emailService.SendBookingConfirmationAsync(
                        user.Email,
                        user.FullName ?? "",
                        eventToBook.Title,
                        booking.BookingId.ToString(),
                        eventToBook.StartDate,
                        booking.Quantity,
                        booking.TotalAmount);
                }

                return RedirectToAction("Payment", new { bookingId = booking.BookingId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating booking: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        public async Task<IActionResult> MyBookings(PaymentStatus? status, int page = 1)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            
            var query = _context.Bookings
                .Include(b => b.Event)
                .Where(b => b.UserId == userId);

            if (status.HasValue)
                query = query.Where(b => b.PaymentStatus == status.Value);

            var pageSize = 10;
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new BookingListViewModel
            {
                Bookings = bookings.Select(b => new BookingItemViewModel
                {
                    BookingId = b.BookingId,
                    EventId = b.EventId,
                    EventTitle = b.Event?.Title ?? "",
                    EventImageUrl = b.Event?.ImageUrl ?? "",
                    EventStartDate = b.Event?.StartDate ?? DateTime.Now,
                    Quantity = b.Quantity,
                    TotalAmount = b.TotalAmount,
                    PaymentStatus = b.PaymentStatus,
                    BookingDate = b.BookingDate,
                    CanCancel = b.PaymentStatus != PaymentStatus.Cancelled && 
                               b.PaymentStatus != PaymentStatus.Refunded &&
                               b.Event != null && b.Event.StartDate > DateTime.Now
                }).ToList(),
                FilterStatus = status,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.User)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

            if (booking == null)
                return NotFound();

            var viewModel = new BookingDetailViewModel
            {
                BookingId = booking.BookingId,
                Event = booking.Event!,
                User = booking.User!,
                Quantity = booking.Quantity,
                TotalAmount = booking.TotalAmount,
                PaymentStatus = booking.PaymentStatus,
                PaymentMethod = booking.PaymentMethod,
                BookingDate = booking.BookingDate,
                Tickets = booking.Tickets ?? new List<DBTicket>(),
                CanCancel = booking.PaymentStatus != PaymentStatus.Cancelled && 
                           booking.PaymentStatus != PaymentStatus.Refunded &&
                           booking.Event.StartDate > DateTime.Now,
                CanViewTickets = booking.PaymentStatus == PaymentStatus.Paid
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                var booking = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

                if (booking == null)
                    return NotFound();

                if (booking.Event == null || booking.Event.StartDate <= DateTime.Now)
                {
                    TempData["Error"] = "Không thể hủy đặt chỗ cho sự kiện đã diễn ra";
                    return RedirectToAction("Details", new { id });
                }

                booking.PaymentStatus = PaymentStatus.Cancelled;
                booking.Event.AvailableSeats += booking.Quantity;
                await _context.SaveChangesAsync();

                // Send cancellation email
                if (booking.User != null)
                {
                    await _emailService.SendBookingCancellationAsync(
                        booking.User.Email,
                        booking.User.FullName ?? "",
                        booking.Event.Title,
                        booking.BookingId.ToString());
                }

                TempData["Success"] = "Đã hủy đặt chỗ thành công";
                return RedirectToAction("MyBookings");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cancelling booking: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Details", new { id });
            }
        }

        public async Task<IActionResult> Payment(int bookingId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null)
                return NotFound();

            var viewModel = new PaymentViewModel
            {
                BookingId = bookingId,
                Amount = booking.TotalAmount,
                EventTitle = booking.Event?.Title,
                Quantity = booking.Quantity,
                EventDate = booking.Event?.StartDate
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            try
            {
                if (model.PaymentMethod == "VNPay")
                {
                    var returnUrl = Url.Action("PaymentCallback", "Booking", null, Request.Scheme);
                    var paymentUrl = _paymentService.CreatePaymentUrl(
                        model.BookingId,
                        model.Amount,
                        $"Thanh toán đặt chỗ #{model.BookingId}",
                        returnUrl ?? "");
                    
                    return Redirect(paymentUrl);
                }
                else if (model.PaymentMethod == "Cash")
                {
                    var booking = await _context.Bookings.FindAsync(model.BookingId);
                    if (booking != null)
                    {
                        booking.PaymentStatus = PaymentStatus.Reserved;
                        booking.PaymentMethod = "Cash";
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("Details", new { id = model.BookingId });
                }

                return RedirectToAction("Payment", new { bookingId = model.BookingId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing payment: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Payment", new { bookingId = model.BookingId });
            }
        }

        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
                
                if (!_paymentService.ValidatePaymentCallback(queryParams))
                {
                    TempData["Error"] = "Giao dịch không hợp lệ";
                    return RedirectToAction("MyBookings");
                }

                var bookingId = int.Parse(queryParams["vnp_TxnRef"]);
                var responseCode = queryParams["vnp_ResponseCode"];
                var isSuccess = responseCode == "00";

                await _paymentService.ProcessPaymentCallbackAsync(bookingId, isSuccess, queryParams.GetValueOrDefault("vnp_TransactionNo", ""));

                if (isSuccess)
                {
                    // Create tickets
                    var booking = await _context.Bookings
                        .Include(b => b.Event)
                        .Include(b => b.User)
                        .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                    if (booking != null)
                    {
                        for (int i = 0; i < booking.Quantity; i++)
                        {
                            var ticketCode = PaymentHelper.GenerateTicketCode();
                            var ticket = new DBTicket
                            {
                                TicketId = 0,
                                BookingID = bookingId,
                                UserID = booking.UserId,
                                TicketCode = ticketCode,
                                QRCodeData = ticketCode,
                                CreatedAt = DateTime.Now
                            };
                            _context.Tickets.Add(ticket);
                        }
                        await _context.SaveChangesAsync();

                        // Send ticket email
                        if (booking.User != null && booking.Event != null)
                        {
                            await _emailService.SendTicketAsync(
                                booking.User.Email,
                                booking.User.FullName ?? "",
                                booking.Event.Title,
                                booking.Tickets?.FirstOrDefault()?.TicketCode ?? "",
                                booking.Event.StartDate);
                        }
                    }

                    TempData["Success"] = "Thanh toán thành công";
                }
                else
                {
                    TempData["Error"] = "Thanh toán thất bại";
                }

                return RedirectToAction("Details", new { id = bookingId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in payment callback: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý thanh toán";
                return RedirectToAction("MyBookings");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutFromCart()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                
                // Get all cart items
                var cartItems = await _context.Carts
                    .Include(c => c.Event)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống";
                    return RedirectToAction("Index", "Cart");
                }

                var user = await _context.Users.FindAsync(userId);
                var createdBookings = new List<int>();

                // Create booking for each cart item
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Event == null)
                        continue;

                    // Check available seats
                    if (cartItem.Event.AvailableSeats < cartItem.Quantity)
                    {
                        TempData["Error"] = $"Không đủ chỗ trống cho sự kiện '{cartItem.Event.Title}'";
                        return RedirectToAction("Index", "Cart");
                    }

                    // Create booking
                    var booking = new DBBooking
                    {
                        BookingId = 0,
                        EventId = cartItem.EventId,
                        UserId = userId,
                        Quantity = cartItem.Quantity,
                        TotalAmount = (decimal)(cartItem.Quantity * cartItem.Event.TicketPrice),
                        PaymentStatus = PaymentStatus.Pending,
                        PaymentMethod = "Pending",
                        BookingDate = DateTime.Now
                    };

                    _context.Bookings.Add(booking);
                    cartItem.Event.AvailableSeats -= cartItem.Quantity;
                    
                    await _context.SaveChangesAsync();
                    createdBookings.Add(booking.BookingId);

                    // Send booking confirmation email
                    if (user != null)
                    {
                        await _emailService.SendBookingConfirmationAsync(
                            user.Email,
                            user.FullName ?? "",
                            cartItem.Event.Title,
                            booking.BookingId.ToString(),
                            cartItem.Event.StartDate,
                            booking.Quantity,
                            booking.TotalAmount);
                    }
                }

                // Clear cart after successful bookings
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã tạo {createdBookings.Count} đơn đặt chỗ thành công";
                return RedirectToAction("MyBookings");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking out from cart: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}

