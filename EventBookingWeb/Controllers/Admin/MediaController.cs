using EventBookingWeb.Attributes;
using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookingWeb.Controllers.Admin
{
    [AuthorizeAdmin]
    public class MediaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(
            ApplicationDbContext context,
            IFileUploadService fileUploadService,
            ILogger<MediaController> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        // Banner Management
        public async Task<IActionResult> Banners()
        {
            var banners = await _context.Banners.OrderByDescending(b => b.CreatedAt).ToListAsync();
            return View(banners);
        }

        public IActionResult CreateBanner()
        {
            return View(new DBBanner { BannerID = 0, IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBanner(DBBanner model, IFormFile? imageFile)
        {
            try
            {
                if (imageFile == null)
                {
                    ModelState.AddModelError("", "Vui lòng chọn hình ảnh");
                    return View(model);
                }

                model.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile, "banners");
                model.CreatedAt = DateTime.Now;

                _context.Banners.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm banner thành công";
                return RedirectToAction("Banners");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating banner: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        public async Task<IActionResult> UpdateBanner(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
                return NotFound();

            return View(banner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBanner(DBBanner model, IFormFile? imageFile)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(model.BannerID);
                if (banner == null)
                    return NotFound();

                if (imageFile != null)
                {
                    if (!string.IsNullOrEmpty(banner.ImageUrl))
                    {
                        await _fileUploadService.DeleteFileAsync(banner.ImageUrl);
                    }
                    banner.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile, "banners");
                }

                banner.LinkUrl = model.LinkUrl;
                banner.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật banner thành công";
                return RedirectToAction("Banners");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating banner: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(id);
                if (banner == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(banner.ImageUrl))
                {
                    await _fileUploadService.DeleteFileAsync(banner.ImageUrl);
                }

                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa banner thành công";
                return RedirectToAction("Banners");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting banner: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Banners");
            }
        }

        // News Management
        public async Task<IActionResult> News()
        {
            var news = await _context.News
                .Include(n => n.Event)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return View(news);
        }

        public async Task<IActionResult> CreateNews()
        {
            ViewBag.Events = await _context.Events.ToListAsync();
            return View(new DBNews { NewsId = 0, IsVisible = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNews(DBNews model, IFormFile? imageFile)
        {
            try
            {
                if (imageFile == null)
                {
                    ModelState.AddModelError("", "Vui lòng chọn hình ảnh");
                    ViewBag.Events = await _context.Events.ToListAsync();
                    return View(model);
                }

                model.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile, "news");
                model.CreatedAt = DateTime.Now;

                _context.News.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm tin tức thành công";
                return RedirectToAction("News");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating news: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                ViewBag.Events = await _context.Events.ToListAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> UpdateNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
                return NotFound();

            ViewBag.Events = await _context.Events.ToListAsync();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNews(DBNews model, IFormFile? imageFile)
        {
            try
            {
                var news = await _context.News.FindAsync(model.NewsId);
                if (news == null)
                    return NotFound();

                if (imageFile != null)
                {
                    if (!string.IsNullOrEmpty(news.ImageUrl))
                    {
                        await _fileUploadService.DeleteFileAsync(news.ImageUrl);
                    }
                    news.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile, "news");
                }

                news.EventId = model.EventId;
                news.Title = model.Title;
                news.NewsContentHtml = model.NewsContentHtml;
                news.IsVisible = model.IsVisible;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật tin tức thành công";
                return RedirectToAction("News");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating news: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
                ViewBag.Events = await _context.Events.ToListAsync();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                var news = await _context.News.FindAsync(id);
                if (news == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(news.ImageUrl))
                {
                    await _fileUploadService.DeleteFileAsync(news.ImageUrl);
                }

                _context.News.Remove(news);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa tin tức thành công";
                return RedirectToAction("News");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting news: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("News");
            }
        }
    }
}

