using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class SeoController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            var values = db.TblSeo.ToList();
            return View(values);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var value = db.TblSeo.Find(id);
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblSeo p)
        {
            var value = db.TblSeo.Find(p.Id);
            value.Title = p.Title;
            value.Description = p.Description;
            value.Keywords = p.Keywords;
            db.SaveChanges();
            CacheHelper.ClearLayoutCache();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblSeo p)
        {
            if (ModelState.IsValid)
            {
                db.TblSeo.Add(p);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                return RedirectToAction("Index");
            }
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var value = db.TblSeo.Find(id);
            if(value != null)
            {
                db.TblSeo.Remove(value);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                return Json(new { success = true, message = "Kayıt silindi." });
            }
            return Json(new { success = false, message = "Kayıt bulunamadı." });
        }
    }
}

