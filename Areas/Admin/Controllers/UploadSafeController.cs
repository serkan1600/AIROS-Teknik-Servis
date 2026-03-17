using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class UploadSafeController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Image(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // 1. Güvenlik Kontrolü: Sadece resim formatları
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!System.Linq.Enumerable.Contains(allowedExtensions, extension))
                {
                    return Json(new { success = false, message = "Sadece resim dosyaları (.jpg, .png, .webp) yüklenebilir." });
                }

                // 2. Güvenlik Kontrolü: Dosya Boyutu (Max 5MB)
                if (file.ContentLength > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "Dosya boyutu 5MB'dan küçük olmalıdır." });
                }

                try
                {
                    // 3. Klasör Kontrolü
                    var uploadDir = Server.MapPath("~/Uploads");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // 4. Güvenli Dosya Adı Oluşturma
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var path = Path.Combine(uploadDir, fileName);

                    // 5. Kaydet (Optimize Ederek)
                    try 
                    {
                        var optimizedPath = AIROSWEB.Helpers.ImageHelper.SaveImageOptimized(file.InputStream, uploadDir, fileName);
                        if (!string.IsNullOrEmpty(optimizedPath)) 
                        {
                            // Eğer optimize edilmişse onu kullan (dosya adı değişmiş olabilir .jpg)
                            fileName = optimizedPath; 
                        }
                        else 
                        {
                             // Optimize edilemedi, orjinalini kaydet
                             file.SaveAs(path);
                        }
                    }
                    catch 
                    {
                        // Hata olursa orjinalini kaydet
                        file.SaveAs(path);
                    }

                    return Json(new { success = true, path = "/Uploads/" + fileName });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Yükleme sırasında bir hata oluştu: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Lütfen bir dosya seçin." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Video(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // 1. Güvenlik Kontrolü: Sadece video formatları
                var allowedExtensions = new[] { ".mp4", ".webm", ".ogg", ".mov" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!System.Linq.Enumerable.Contains(allowedExtensions, extension))
                {
                    return Json(new { success = false, message = "Sadece video dosyaları (.mp4, .webm, .ogg, .mov) yüklenebilir." });
                }

                // 2. Güvenlik Kontrolü: Dosya Boyutu (Max 100MB)
                if (file.ContentLength > 100 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "Video boyutu 100MB'dan küçük olmalıdır." });
                }

                try
                {
                    // 3. Klasör Kontrolü
                    var uploadDir = Server.MapPath("~/Uploads");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // 4. Güvenli Dosya Adı Oluşturma
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var path = Path.Combine(uploadDir, fileName);

                    // 5. Kaydet
                    file.SaveAs(path);

                    return Json(new { success = true, path = "/Uploads/" + fileName });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Yükleme sırasında bir hata oluştu: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Lütfen bir video dosyası seçin." });
        }
        [HttpPost]
        public ActionResult CkEditorUpload()
        {
            var upload = Request.Files["upload"]; // CKEditor her zaman bu isimle gönderir
            string CKEditorFuncNum = Request.QueryString["CKEditorFuncNum"];
            string url = "";
            string error = "";

            if (upload != null && upload.ContentLength > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var extension = Path.GetExtension(upload.FileName).ToLower();

                if (!System.Linq.Enumerable.Contains(allowedExtensions, extension))
                {
                    error = "Geçersiz dosya formatı. Sadece resim yükleyin.";
                }
                else
                {
                    try
                    {
                        var uploadDir = Server.MapPath("~/Uploads/SayfaResimleri");
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var path = Path.Combine(uploadDir, fileName);
                        
                        upload.SaveAs(path);
                        url = "/Uploads/SayfaResimleri/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        error = "Yükleme sırasında hata: " + ex.Message;
                    }
                }
            }
            else
            {
                error = "Lütfen geçerli bir dosya seçin.";
            }

            // CKEditor resim yükleme diyalog kutusu klasik IFrame ('form') arıyorsa:
            if (!string.IsNullOrEmpty(CKEditorFuncNum))
            {
                string script = $"<script type='text/javascript'>window.parent.CKEDITOR.tools.callFunction({CKEditorFuncNum}, '{url}', '{error}');</script>";
                return Content(script, "text/html");
            }

            // Sürükle bırak / XHR ile JSON bekliyorsa (Yeni Nesil):
            if (string.IsNullOrEmpty(error))
            {
                return Json(new { uploaded = 1, fileName = Path.GetFileName(url), url = url });
            }
            else
            {
                return Json(new { uploaded = 0, error = new { message = error } });
            }
        }
    }
}
