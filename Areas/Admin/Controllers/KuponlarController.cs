using System;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class KuponlarController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            var list = db.TblKuponlar.OrderByDescending(x => x.Id).ToList();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblKuponlar model)
        {
            if (ModelState.IsValid)
            {
                model.Kod = model.Kod.ToUpper().Trim();
                if (db.TblKuponlar.Any(x => x.Kod == model.Kod))
                {
                    TempData["Error"] = "Bu kupon kodu zaten mevcut.";
                    return RedirectToAction("Index");
                }

                model.AktifMi = true;
                model.KullanimSayisi = 0;
                db.TblKuponlar.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Yeni kupon başarıyla oluşturuldu.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ToggleStatus(int id)
        {
            var kupon = db.TblKuponlar.Find(id);
            if (kupon != null)
            {
                kupon.AktifMi = !kupon.AktifMi;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var kupon = db.TblKuponlar.Find(id);
            if (kupon != null)
            {
                db.TblKuponlar.Remove(kupon);
                db.SaveChanges();
                TempData["Success"] = "Kupon başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
