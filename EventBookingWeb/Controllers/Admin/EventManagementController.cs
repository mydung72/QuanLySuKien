using EventBookingWeb.Attributes;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using EventBookingWeb.Services;
using EventBookingWeb.ViewModels.Event;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers.Admin
{
    [AuthorizeAdmin]
    public class EventManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<EventManagementController> _logger;

        public EventManagementController(
            ApplicationDbContext context,
            IFileUploadService fileUploadService,
            ILogger<EventManagementController> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.CategoryEvents)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();

                return View(events);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading events: {ex.Message}");
                return View(new List<DBEvent>());
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.CategoryEvents.ToListAsync();
            return View(new EventCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await _context.CategoryEvents.ToListAsync();
                    return View(model);
                }

                string imageUrl = "/images/default-event.jpg";
                if (model.PosterImage != null)
                {
                    imageUrl = await _fileUploadService.UploadFileAsync(model.PosterImage, "events");
                }

                var eventItem = new DBEvent
                {
                    EventId = 0,
                    EventCategoryId = model.SelectedCategoryIds,
                    Title = model.Title,
                    EventDescription = model.EventDescription,
                    EventContentHtml = model.EventContentHtml,
                    Location = model.Location,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    TotalSeats = model.TotalSeats,
                    AvailableSeats = model.TotalSeats,
                    TicketPrice = model.TicketPrice,
                    ImageUrl = imageUrl,
                    EventStatus = model.EventStatus,
                    CreatedAt = DateTime.Now
                };

                _context.Events.Add(eventItem);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Tạo sự kiện thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating event: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                ViewBag.Categories = await _context.CategoryEvents.ToListAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.CategoryEvents)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null)
                return NotFound();

            var viewModel = new EventCreateViewModel
            {
                EventId = eventItem.EventId,
                Title = eventItem.Title,
                EventDescription = eventItem.EventDescription,
                EventContentHtml = eventItem.EventContentHtml,
                Location = eventItem.Location,
                StartDate = eventItem.StartDate,
                EndDate = eventItem.EndDate,
                TotalSeats = eventItem.TotalSeats,
                TicketPrice = eventItem.TicketPrice,
                EventStatus = eventItem.EventStatus,
                ExistingImageUrl = eventItem.ImageUrl,
                SelectedCategoryIds = eventItem.CategoryEvents?.Select(c => c.CategoryEventId).ToList() ?? new List<int>()
            };

            ViewBag.Categories = await _context.CategoryEvents.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await _context.CategoryEvents.ToListAsync();
                    return View(model);
                }

                var eventItem = await _context.Events.FindAsync(model.EventId);
                if (eventItem == null)
                    return NotFound();

                if (model.PosterImage != null)
                {
                    if (!string.IsNullOrEmpty(eventItem.ImageUrl))
                    {
                        await _fileUploadService.DeleteFileAsync(eventItem.ImageUrl);
                    }
                    eventItem.ImageUrl = await _fileUploadService.UploadFileAsync(model.PosterImage, "events");
                }

                eventItem.Title = model.Title;
                eventItem.EventDescription = model.EventDescription;
                eventItem.EventContentHtml = model.EventContentHtml;
                eventItem.Location = model.Location;
                eventItem.StartDate = model.StartDate;
                eventItem.EndDate = model.EndDate;
                eventItem.TotalSeats = model.TotalSeats;
                eventItem.TicketPrice = model.TicketPrice;
                eventItem.EventStatus = model.EventStatus;
                eventItem.EventCategoryId = model.SelectedCategoryIds;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật sự kiện thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating event: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                ViewBag.Categories = await _context.CategoryEvents.ToListAsync();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var eventItem = await _context.Events
                    .Include(e => e.NewsList)
                    .FirstOrDefaultAsync(e => e.EventId == id);

                if (eventItem == null)
                    return NotFound();

                var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
                if (hasBookings)
                {
                    TempData["Error"] = "Không thể xóa sự kiện đang có đặt chỗ";
                    return RedirectToAction("Index");
                }

                if (!string.IsNullOrEmpty(eventItem.ImageUrl))
                {
                    await _fileUploadService.DeleteFileAsync(eventItem.ImageUrl);
                }

                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa sự kiện thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting event: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, EventStatus status)
        {
            try
            {
                var eventItem = await _context.Events.FindAsync(id);
                if (eventItem == null)
                    return NotFound();

                eventItem.EventStatus = status;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật trạng thái thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating event status: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }
    }
}

