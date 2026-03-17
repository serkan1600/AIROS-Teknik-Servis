using System;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class YorumlarController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            var values = db.TblMusteriYorumlari.OrderByDescending(x => x.Id).ToList();
            return View(values);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblMusteriYorumlari p)
        {
            if (ModelState.IsValid)
            {
                p.Tarih = DateTime.Now;
                p.AktifMi = true; // Varsayılan aktif
                db.TblMusteriYorumlari.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(p);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var value = db.TblMusteriYorumlari.Find(id);
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblMusteriYorumlari p)
        {
            if (ModelState.IsValid)
            {
                var value = db.TblMusteriYorumlari.Find(p.Id);
                if (value != null)
                {
                    value.AdSoyad = p.AdSoyad;
                    value.Meslek = p.Meslek; // Unvan düzeltildi
                    value.Yorum = p.Yorum;
                    value.Puan = p.Puan;
                    value.ResimYolu = p.ResimYolu; // Resim düzeltildi
                    value.AktifMi = p.AktifMi;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var value = db.TblMusteriYorumlari.Find(id);
            if(value != null)
            {
                db.TblMusteriYorumlari.Remove(value);
                db.SaveChanges();
                TempData["Success"] = "Yorum silindi.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DurumDegistir(int id)
        {
            var value = db.TblMusteriYorumlari.Find(id);
            if(value != null)
            {
                value.AktifMi = !value.AktifMi;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
