using System.Collections.Generic;
using System.Web.Mvc;
using AIROSWEB.BLL.Managers;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class HizmetlerController : Controller
    {
        HizmetManager manager = new HizmetManager();

        public ActionResult Index()
        {
            var values = manager.TumHizmetleriGetir();
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
        public ActionResult Create(TblHizmetler p)
        {
            if(string.IsNullOrEmpty(p.Baslik))
            {
                ViewBag.Error = "Başlık boş geçilemez!";
                return View(p);
            }
            p.AktifMi = true;
            manager.HizmetEkle(p);
            CacheHelper.ClearLayoutCache();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var value = manager.HizmetBul(id);
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblHizmetler p)
        {
            if (ModelState.IsValid)
            {
                var value = manager.HizmetBul(p.Id);
                if (value != null)
                {
                    value.Baslik = p.Baslik;
                    value.Aciklama = p.Aciklama;
                    value.Ikon = p.Ikon;
                    value.Icerik = p.Icerik; 
                    value.ResimYolu = p.ResimYolu;
                    value.MetaTitle = p.MetaTitle;
                    value.MetaDescription = p.MetaDescription;
                    value.Keywords = p.Keywords;
                    value.AktifMi = p.AktifMi;
                    manager.HizmetGuncelle(value);
                    CacheHelper.ClearLayoutCache();
                    return RedirectToAction("Index");
                }
            }
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            manager.HizmetSil(id);
            CacheHelper.ClearLayoutCache();
            return Json(new { success = true, message = "Hizmet başarıyla silindi." });
        }
    }
}

