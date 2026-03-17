using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using System.Data.Entity;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class HizmetBolgeleriController : Controller
    {
        private AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/HizmetBolgeleri
        public ActionResult Index()
        {
            var model = db.TblHizmetBolgeleri.OrderBy(x => x.Sira).ToList();
            return View(model);
        }

        // GET: Admin/HizmetBolgeleri/Create
        public ActionResult Create()
        {
            return View(new TblHizmetBolgeleri { AktifMi = true, Sira = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblHizmetBolgeleri model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Slug))
                {
                    model.Slug = AIROSWEB.Helpers.SeoHelper.ToSeoUrl(model.BolgeAdi);
                }
                db.TblHizmetBolgeleri.Add(model);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                TempData["Success"] = "Hizmet bölgesi başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/HizmetBolgeleri/Edit/5
        public ActionResult Edit(int id)
        {
            var model = db.TblHizmetBolgeleri.Find(id);
            if (model == null) return HttpNotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblHizmetBolgeleri model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Slug))
                {
                    model.Slug = AIROSWEB.Helpers.SeoHelper.ToSeoUrl(model.BolgeAdi);
                }
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                TempData["Success"] = "Hizmet bölgesi güncellendi.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var model = db.TblHizmetBolgeleri.Find(id);
            if (model != null)
            {
                db.TblHizmetBolgeleri.Remove(model);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                TempData["Success"] = "Hizmet bölgesi silindi.";
            }
            return RedirectToAction("Index");
        }

    }
}
