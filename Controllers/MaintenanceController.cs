using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Controllers
{
    public class MaintenanceController : BaseController
    {
        // GET: Maintenance
        public ActionResult Index()
        {
            var model = GetBaseModel("Bakım");
            var ayar = model.Ayarlar;
            
            if (ayar == null || !ayar.BakimModu)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = ayar.BakimMesaji ?? "Sizlere daha iyi hizmet verebilmek için çalışıyoruz. Lütfen daha sonra tekrar deneyiniz.";
            return View(model);
        }
    }
}
