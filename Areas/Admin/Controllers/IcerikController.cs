using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class IcerikController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index(string bolum = null)
        {
            var values = db.TblIcerikler.AsQueryable();
            
            if (!string.IsNullOrEmpty(bolum))
            {
                values = values.Where(x => x.BolumAdi == bolum);
                ViewBag.Title = bolum + " Yönetimi";
            }
            
            return View(values.OrderBy(x => x.BolumAdi).ThenBy(x => x.Sira).ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblIcerikler p)
        {
            if (ModelState.IsValid)
            {
                p.AktifMi = true;
                db.TblIcerikler.Add(p);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                return RedirectToAction("Index", new { bolum = p.BolumAdi });
            }
            return View(p);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var value = db.TblIcerikler.Find(id);
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblIcerikler p)
        {
            var value = db.TblIcerikler.Find(p.Id);
            value.Baslik = p.Baslik;
            value.AltBaslik = p.AltBaslik;
            value.Aciklama = p.Aciklama;
            value.ResimYolu = p.ResimYolu;
            value.BolumAdi = p.BolumAdi;
            value.Sira = p.Sira;
            value.AktifMi = p.AktifMi;
            db.SaveChanges();
            CacheHelper.ClearLayoutCache();
            return RedirectToAction("Index", new { bolum = value.BolumAdi });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var value = db.TblIcerikler.Find(id);
            if (value != null)
            {
                var bolum = value.BolumAdi;
                db.TblIcerikler.Remove(value);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                TempData["Success"] = "İçerik silindi.";
                return RedirectToAction("Index", new { bolum = bolum });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DurumDegistir(int id)
        {
            var value = db.TblIcerikler.Find(id);
            if (value != null)
            {
                value.AktifMi = !value.AktifMi;
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                TempData["Success"] = "İçerik durumu güncellendi.";
            }
            return RedirectToAction("Index", new { bolum = value?.BolumAdi });
        }
    }
}

