using EventBookingWeb.Attributes;
using EventBookingWeb.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers.Admin
{
    [AuthorizeAdmin]
    public class CategoryManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryManagementController> _logger;

        public CategoryManagementController(ApplicationDbContext context, ILogger<CategoryManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _context.CategoryEvents
                    .Include(c => c.Events)
                    .OrderBy(c => c.CategoryEventName)
                    .ToListAsync();

                return View("~/Views/Admin/CategoryManagement/Index.cshtml", categories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading categories: {ex.Message}");
                return View("~/Views/Admin/CategoryManagement/Index.cshtml", new List<DBCategoryEvent>());
            }
        }

        public IActionResult Create()
        {
            return View("~/Views/Admin/CategoryManagement/Create.cshtml", new DBCategoryEvent 
            { 
                CategoryEventId = 0, 
                CategoryEventName = "" 
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DBCategoryEvent model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.CategoryEventName))
                {
                    ModelState.AddModelError("CategoryEventName", "Tên danh mục là bắt buộc");
                    return View("~/Views/Admin/CategoryManagement/Create.cshtml", model);
                }

                if (await _context.CategoryEvents.AnyAsync(c => c.CategoryEventName == model.CategoryEventName))
                {
                    ModelState.AddModelError("CategoryEventName", "Danh mục này đã tồn tại");
                    return View("~/Views/Admin/CategoryManagement/Create.cshtml", model);
                }

                _context.CategoryEvents.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View("~/Views/Admin/CategoryManagement/Create.cshtml", model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.CategoryEvents.FindAsync(id);
            if (category == null)
                return NotFound();

            return View("~/Views/Admin/CategoryManagement/Edit.cshtml", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DBCategoryEvent model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.CategoryEventName))
                {
                    ModelState.AddModelError("CategoryEventName", "Tên danh mục là bắt buộc");
                    return View("~/Views/Admin/CategoryManagement/Edit.cshtml", model);
                }

                var category = await _context.CategoryEvents.FindAsync(model.CategoryEventId);
                if (category == null)
                    return NotFound();

                if (await _context.CategoryEvents.AnyAsync(c => c.CategoryEventName == model.CategoryEventName && c.CategoryEventId != model.CategoryEventId))
                {
                    ModelState.AddModelError("CategoryEventName", "Danh mục này đã tồn tại");
                    return View("~/Views/Admin/CategoryManagement/Edit.cshtml", model);
                }

                category.CategoryEventName = model.CategoryEventName;
                category.CategoryEventDescription = model.CategoryEventDescription;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating category: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View("~/Views/Admin/CategoryManagement/Edit.cshtml", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.CategoryEvents
                    .Include(c => c.Events)
                    .FirstOrDefaultAsync(c => c.CategoryEventId == id);

                if (category == null)
                    return NotFound();

                if (category.Events != null && category.Events.Any())
                {
                    TempData["Error"] = "Không thể xóa danh mục đang có sự kiện";
                    return RedirectToAction("Index");
                }

                _context.CategoryEvents.Remove(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa danh mục thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }
    }
}

