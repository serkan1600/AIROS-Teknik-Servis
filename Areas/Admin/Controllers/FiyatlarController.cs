using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using System.Data.Entity;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class FiyatlarController : Controller
    {
        private AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            var values = db.TblArizaFiyatlari.OrderBy(x => x.CihazTuru).ThenBy(x => x.Sira).ToList();
            return View(values);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Cihazlar = db.TblArizaFiyatlari.Select(x => x.CihazTuru).Distinct().ToList();
            return View(new TblArizaFiyatlari { AktifMi = true, Sira = 99 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblArizaFiyatlari p)
        {
            if (ModelState.IsValid)
            {
                db.TblArizaFiyatlari.Add(p);
                db.SaveChanges();
                TempData["Success"] = "Arıza/Fiyat başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            return View(p);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var value = db.TblArizaFiyatlari.Find(id);
            if (value == null) return HttpNotFound();
            ViewBag.Cihazlar = db.TblArizaFiyatlari.Select(x => x.CihazTuru).Distinct().ToList();
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblArizaFiyatlari p)
        {
            if (ModelState.IsValid)
            {
                var value = db.TblArizaFiyatlari.Find(p.Id);
                if (value == null) return HttpNotFound();

                value.CihazTuru = p.CihazTuru;
                value.Marka = p.Marka;
                value.ArizaAdi = p.ArizaAdi;
                value.FiyatAraligi = p.FiyatAraligi;
                value.Sira = p.Sira;
                value.AktifMi = p.AktifMi;

                db.SaveChanges();
                TempData["Success"] = "Arıza/Fiyat güncellendi.";
                return RedirectToAction("Index");
            }
            return View(p);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var value = db.TblArizaFiyatlari.Find(id);
                if (value != null)
                {
                    db.TblArizaFiyatlari.Remove(value);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Kayıt başarıyla silindi." });
                }
                return Json(new { success = false, message = "Kayıt bulunamadı." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Silme Hatası: " + ex.Message });
            }
        }
        
        [HttpPost]
        public ActionResult ChangeStatus(int id)
        {
            try
            {
                var value = db.TblArizaFiyatlari.Find(id);
                if (value != null)
                {
                    value.AktifMi = !value.AktifMi;
                    db.SaveChanges();
                    return Json(new { success = true, state = value.AktifMi, message = "Durum güncellendi." });
                }
                return Json(new { success = false, message = "Kayıt bulunamadı." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }
    }
}
