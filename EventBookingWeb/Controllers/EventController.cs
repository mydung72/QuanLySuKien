using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.ViewModels.Event;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventController> _logger;

        public EventController(ApplicationDbContext context, ILogger<EventController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(EventFilterViewModel filter, int page = 1)
        {
            try
            {
                var query = _context.Events
                    .Include(e => e.CategoryEvents)
                    .AsQueryable();

                // Apply filters
                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(e => e.CategoryEvents!.Any(c => c.CategoryEventId == filter.CategoryId.Value));
                }

                if (filter.StartDate.HasValue)
                {
                    query = query.Where(e => e.StartDate >= filter.StartDate.Value);
                }

                if (filter.EndDate.HasValue)
                {
                    query = query.Where(e => e.EndDate <= filter.EndDate.Value);
                }

                if (!string.IsNullOrEmpty(filter.Location))
                {
                    query = query.Where(e => e.Location.Contains(filter.Location));
                }

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(e => e.Title.Contains(filter.SearchTerm) || 
                                           (e.EventDescription != null && e.EventDescription.Contains(filter.SearchTerm)));
                }

                // Sorting
                query = filter.SortBy switch
                {
                    "date_asc" => query.OrderBy(e => e.StartDate),
                    "date_desc" => query.OrderByDescending(e => e.StartDate),
                    "price_asc" => query.OrderBy(e => e.TicketPrice),
                    "price_desc" => query.OrderByDescending(e => e.TicketPrice),
                    _ => query.OrderBy(e => e.StartDate)
                };

                var pageSize = 12;
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var events = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new EventListViewModel
                {
                    Events = events,
                    Filter = filter,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                ViewBag.Categories = await _context.CategoryEvents.ToListAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading events: {ex.Message}");
                return View(new EventListViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var eventItem = await _context.Events
                    .Include(e => e.CategoryEvents)
                    .Include(e => e.NewsList)
                    .FirstOrDefaultAsync(e => e.EventId == id);

                if (eventItem == null)
                {
                    return NotFound();
                }

                var viewModel = new EventDetailViewModel
                {
                    Event = eventItem,
                    Categories = eventItem.CategoryEvents ?? new List<DBCategoryEvent>(),
                    RelatedNews = eventItem.NewsList ?? new List<DBNews>(),
                    CanBook = eventItem.AvailableSeats > 0 && eventItem.StartDate > DateTime.Now,
                    IsLoggedIn = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"))
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading event details: {ex.Message}");
                return NotFound();
            }
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var filter = new EventFilterViewModel { SearchTerm = keyword };
            return RedirectToAction("Index", filter);
        }
    }
}

