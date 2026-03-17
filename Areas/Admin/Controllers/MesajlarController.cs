using System;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class MesajlarController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Mesajlar
        public ActionResult Index(string durum = "tumu")
        {
            var mesajlar = db.TblMesajlar.AsQueryable();

            switch (durum)
            {
                case "okunmamis":
                    mesajlar = mesajlar.Where(x => !x.Okundu);
                    ViewBag.Title = "Okunmamış Mesajlar";
                    break;
                case "yanitlanmamis":
                    mesajlar = mesajlar.Where(x => !x.Yanitlandi);
                    ViewBag.Title = "Yanıtlanmamış Mesajlar";
                    break;
                default:
                    ViewBag.Title = "Tüm Mesajlar";
                    break;
            }

            ViewBag.OkunmamisSayisi = db.TblMesajlar.Count(x => !x.Okundu);
            ViewBag.YanitlanmamisSayisi = db.TblMesajlar.Count(x => !x.Yanitlandi);
            ViewBag.ToplamSayisi = db.TblMesajlar.Count();

            return View(mesajlar.OrderByDescending(x => x.Tarih).ToList());
        }

        // GET: Admin/Mesajlar/Details/5
        public ActionResult Details(int id)
        {
            var mesaj = db.TblMesajlar.Find(id);
            if (mesaj == null)
            {
                return HttpNotFound();
            }

            // Mesajı okundu olarak işaretle
            if (!mesaj.Okundu)
            {
                mesaj.Okundu = true;
                db.SaveChanges();
            }

            return View(mesaj);
        }

        // POST: Admin/Mesajlar/ToggleYanitlandi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleYanitlandi(int id)
        {
            var mesaj = db.TblMesajlar.Find(id);
            if (mesaj != null)
            {
                mesaj.Yanitlandi = !mesaj.Yanitlandi;
                db.SaveChanges();
                TempData["Success"] = "Durum güncellendi.";
            }
            return RedirectToAction("Details", new { id });
        }

        // POST: Admin/Mesajlar/SaveNote
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SaveNote(int id, string yoneticiNotu)
        {
            var mesaj = db.TblMesajlar.Find(id);
            if (mesaj != null)
            {
                mesaj.YoneticiNotu = yoneticiNotu;
                db.SaveChanges();
                TempData["Success"] = "Not kaydedildi.";
            }
            return RedirectToAction("Details", new { id });
        }

        // POST: Admin/Mesajlar/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var mesaj = db.TblMesajlar.Find(id);
            if (mesaj != null)
            {
                db.TblMesajlar.Remove(mesaj);
                db.SaveChanges();
                TempData["Success"] = "Mesaj silindi.";
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
