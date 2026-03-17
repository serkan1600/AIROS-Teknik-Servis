using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class OptimizationController : Controller
    {
        // POST: Admin/Optimization/OptimizeImages
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OptimizeImages()
        {
            try
            {
                var uploadsPath = Server.MapPath("~/Uploads");
                
                if (!Directory.Exists(uploadsPath))
                {
                    TempData["Error"] = "Uploads klasörü bulunamadı.";
                    return RedirectToAction("Index", "Dashboard");
                }

                // Tüm resimleri optimize et
                int optimizedCount = ImageOptimizer.OptimizeFolder(uploadsPath, maxWidth: 1920, quality: 85);

                TempData["Success"] = $"✅ {optimizedCount} resim başarıyla optimize edildi! Site artık çok daha hızlı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Resim optimizasyonu sırasında hata: " + ex.Message;
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: Admin/Optimization/GetImageStats
        public JsonResult GetImageStats()
        {
            try
            {
                var uploadsPath = Server.MapPath("~/Uploads");
                var extensions = new[] { ".jpg", ".jpeg", ".png" };
                
                var files = Directory.GetFiles(uploadsPath)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                    .Select(f => new FileInfo(f))
                    .ToList();

                var totalSize = files.Sum(f => f.Length);
                var largeFiles = files.Where(f => f.Length > 500 * 1024).Count();

                return Json(new
                {
                    totalFiles = files.Count,
                    totalSizeMB = Math.Round(totalSize / (1024.0 * 1024.0), 2),
                    largeFiles = largeFiles,
                    largestFile = files.OrderByDescending(f => f.Length).FirstOrDefault()?.Name,
                    largestSizeMB = Math.Round(files.OrderByDescending(f => f.Length).FirstOrDefault()?.Length / (1024.0 * 1024.0) ?? 0, 2)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
