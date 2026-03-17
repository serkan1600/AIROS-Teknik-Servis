using System;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class SosyalKanitController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            var list = db.TblSosyalKanit.OrderBy(x => x.Sira).ToList();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblSosyalKanit model)
        {
            if (ModelState.IsValid)
            {
                model.AktifMi = true;
                if(model.Sira == 0) model.Sira = 1;
                model.OlusturulmaTarihi = DateTime.Now;
                db.TblSosyalKanit.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Yeni Sosyal Kanıt bildirimi başarıyla oluşturuldu.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var item = db.TblSosyalKanit.Find(id);
            if (item == null) return HttpNotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblSosyalKanit model)
        {
            if (ModelState.IsValid)
            {
                var existing = db.TblSosyalKanit.Find(model.Id);
                if (existing != null)
                {
                    existing.MusteriAdi = model.MusteriAdi;
                    existing.Lokasyon = model.Lokasyon;
                    existing.Islem = model.Islem;
                    existing.Tip = model.Tip;
                    existing.Sira = model.Sira;
                    // Eğer kullanıcı zamanı değiştirirse ona da izin verelim
                    existing.Zaman = model.Zaman;
                    existing.AktifMi = model.AktifMi;
                    
                    db.SaveChanges();
                    
                    // Cache temizle
                    foreach(System.Collections.DictionaryEntry entry in System.Web.HttpRuntime.Cache)
                    {
                        if (entry.Key.ToString().StartsWith("LayoutModel_"))
                        {
                            System.Web.HttpRuntime.Cache.Remove(entry.Key.ToString());
                        }
                    }

                    TempData["Success"] = "Sosyal Kanıt bildirimi başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ToggleStatus(int id)
        {
            var item = db.TblSosyalKanit.Find(id);
            if (item != null)
            {
                item.AktifMi = !item.AktifMi;
                db.SaveChanges();
                
                // Cache temizle
                foreach(System.Collections.DictionaryEntry entry in System.Web.HttpRuntime.Cache)
                {
                    if (entry.Key.ToString().StartsWith("LayoutModel_"))
                    {
                        System.Web.HttpRuntime.Cache.Remove(entry.Key.ToString());
                    }
                }

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var item = db.TblSosyalKanit.Find(id);
            if (item != null)
            {
                db.TblSosyalKanit.Remove(item);
                db.SaveChanges();
                
                // Cache temizle
                foreach(System.Collections.DictionaryEntry entry in System.Web.HttpRuntime.Cache)
                {
                    if (entry.Key.ToString().StartsWith("LayoutModel_"))
                    {
                        System.Web.HttpRuntime.Cache.Remove(entry.Key.ToString());
                    }
                }

                TempData["Success"] = "Sosyal Kanıt başarıyla silindi.";
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
