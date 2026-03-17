using System;
using System.Web.Mvc;

namespace AIROSWEB.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error/Index (500)
        public ActionResult Index()
        {
            Response.StatusCode = 500;
            var model = GetBaseModel("Hata");
            ViewBag.ErrorTitle = "Sistem Hatası";
            ViewBag.ErrorMessage = "Sunucularımızda beklenen bir güncelleme veya geçici bir sorun nedeniyle bir hata oluştu. Lütfen biraz sonra tekrar deneyin.";
            return View("~/Views/Shared/ErrorPage.cshtml", model);
        }

        // GET: Error/NotFound (404)
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            var model = GetBaseModel("404");
            ViewBag.ErrorTitle = "Sayfa Bulunamadı";
            ViewBag.ErrorMessage = "Aradığınız sayfa silinmiş, taşınmış veya adresi yanlış olabilir. Lütfen anasayfaya dönmek için aşağıdaki butonu kullanın.";
            return View("~/Views/Shared/ErrorPage.cshtml", model);
        }
    }
}
