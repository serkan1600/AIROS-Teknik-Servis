using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class GaleriController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Galeri
        public ActionResult Index()
        {
            var galeri = db.TblGaleri.OrderByDescending(x => x.Tarih).ToList();
            return View(galeri);
        }

        // GET: Admin/Galeri/Create
        public ActionResult Create()
        {
            var existingCategories = db.TblGaleri.Select(x => x.Kategori).Distinct().ToList();
            var defaultCategories = new List<string> { "Kombi", "Klima", "Petek", "Beyaz Eşya", "Diğer" };
            ViewBag.Kategoriler = existingCategories.Union(defaultCategories).OrderBy(x => x).ToList();
            return View();
        }

        // POST: Admin/Galeri/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblGaleri model)
        {
            if (ModelState.IsValid)
            {
                model.Tarih = DateTime.Now;
                db.TblGaleri.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Galeri öğesi başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            
            var existingCategories = db.TblGaleri.Select(x => x.Kategori).Distinct().ToList();
            var defaultCategories = new List<string> { "Kombi", "Klima", "Petek", "Beyaz Eşya", "Diğer" };
            ViewBag.Kategoriler = existingCategories.Union(defaultCategories).OrderBy(x => x).ToList();
            return View(model);
        }

        // GET: Admin/Galeri/Edit/5
        public ActionResult Edit(int id)
        {
            var galeri = db.TblGaleri.Find(id);
            if (galeri == null)
            {
                return HttpNotFound();
            }

            var existingCategories = db.TblGaleri.Select(x => x.Kategori).Distinct().ToList();
            var defaultCategories = new List<string> { "Kombi", "Klima", "Petek", "Beyaz Eşya", "Diğer" };
            ViewBag.Kategoriler = existingCategories.Union(defaultCategories).OrderBy(x => x).ToList();

            return View(galeri);
        }

        // POST: Admin/Galeri/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblGaleri model)
        {
            if (ModelState.IsValid)
            {
                var galeri = db.TblGaleri.Find(model.Id);
                if (galeri != null)
                {
                    galeri.Baslik = model.Baslik;
                    galeri.Aciklama = model.Aciklama;
                    galeri.ResimYolu = model.ResimYolu;
                    galeri.Kategori = model.Kategori;
                    galeri.Konum = model.Konum;
                    galeri.Sira = model.Sira;
                    galeri.AktifMi = model.AktifMi;
                    db.SaveChanges();
                    TempData["Success"] = "Galeri öğesi güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            
            var existingCategories = db.TblGaleri.Select(x => x.Kategori).Distinct().ToList();
            var defaultCategories = new List<string> { "Kombi", "Klima", "Petek", "Beyaz Eşya", "Diğer" };
            ViewBag.Kategoriler = existingCategories.Union(defaultCategories).OrderBy(x => x).ToList();
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var galeri = db.TblGaleri.Find(id);
            if (galeri != null)
            {
                db.TblGaleri.Remove(galeri);
                db.SaveChanges();
                TempData["Success"] = "Galeri öğesi silindi.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DurumDegistir(int id)
        {
            var galeri = db.TblGaleri.Find(id);
            if (galeri != null)
            {
                galeri.AktifMi = !galeri.AktifMi;
                db.SaveChanges();
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
