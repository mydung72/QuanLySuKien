using EventBookingWeb.Attributes;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers
{
    [AuthorizeUser]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(ApplicationDbContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                
                var cartItems = await _context.Carts
                    .Include(c => c.Event)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var viewModel = new CartViewModel
                {
                    Items = cartItems.Select(c => new CartItemViewModel
                    {
                        CartId = c.CartId,
                        EventId = c.EventId,
                        EventTitle = c.Event?.Title ?? "",
                        EventImageUrl = c.Event?.ImageUrl ?? "",
                        EventStartDate = c.Event?.StartDate ?? DateTime.Now,
                        Location = c.Event?.Location ?? "",
                        TicketPrice = c.Event?.TicketPrice ?? 0,
                        Quantity = c.Quantity,
                        AvailableSeats = c.Event?.AvailableSeats ?? 0
                    }).ToList()
                };

                viewModel.TotalItems = viewModel.Items.Sum(i => i.Quantity);
                viewModel.TotalAmount = viewModel.Items.Sum(i => i.SubTotal);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading cart: {ex.Message}");
                return View(new CartViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int eventId, int quantity = 1)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                var eventItem = await _context.Events.FindAsync(eventId);

                if (eventItem == null)
                {
                    return Json(new { success = false, message = "Sự kiện không tồn tại" });
                }

                if (eventItem.AvailableSeats < quantity)
                {
                    return Json(new { success = false, message = "Không đủ chỗ trống" });
                }

                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.EventId == eventId);

                if (existingCart != null)
                {
                    if (existingCart.Quantity + quantity > eventItem.AvailableSeats)
                    {
                        return Json(new { success = false, message = "Không đủ chỗ trống" });
                    }
                    existingCart.Quantity += quantity;
                    existingCart.UpdatedAt = DateTime.Now;
                }
                else
                {
                    var cartItem = new DBCart
                    {
                        CartId = 0,
                        UserId = userId,
                        EventId = eventId,
                        Quantity = quantity,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                var cartCount = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);

                return Json(new { success = true, message = "Đã thêm vào giỏ hàng", cartCount });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding to cart: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartId, int quantity)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

                var cartItem = await _context.Carts
                    .Include(c => c.Event)
                    .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                if (quantity <= 0)
                {
                    _context.Carts.Remove(cartItem);
                }
                else
                {
                    if (cartItem.Event != null && quantity > cartItem.Event.AvailableSeats)
                    {
                        return Json(new { success = false, message = "Không đủ chỗ trống" });
                    }

                    cartItem.Quantity = quantity;
                    cartItem.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                // 🔥 LẤY GIỎ HÀNG SAU KHI UPDATE
                var cartItems = await _context.Carts
                    .Include(c => c.Event)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var totalItems = cartItems.Sum(c => c.Quantity);
                var totalAmount = cartItems.Sum(c =>
                    c.Event != null ? c.Quantity * c.Event.TicketPrice : 0
                );

                var subTotal = cartItem.Event != null
                    ? quantity * cartItem.Event.TicketPrice
                    : 0;

                return Json(new
                {
                    success = true,
                    cartCount = totalItems,
                    subTotal,
                    totalItems,
                    totalAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating cart: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartId)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                var cartItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();

                var cartCount = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);

                return Json(new { success = true, message = "Đã xóa khỏi giỏ hàng", cartCount });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing from cart: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã xóa tất cả sản phẩm khỏi giỏ hàng";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error clearing cart: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                var count = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);

                return Json(new { count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }
    }
}

