using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class SSSController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/SSS
        public ActionResult Index()
        {
            var sss = db.TblSSS.OrderBy(x => x.Sira).ToList();
            return View(sss);
        }

        // GET: Admin/SSS/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/SSS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblSSS model)
        {
            if (ModelState.IsValid)
            {
                db.TblSSS.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Soru başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/SSS/Edit/5
        public ActionResult Edit(int id)
        {
            var sss = db.TblSSS.Find(id);
            if (sss == null)
            {
                return HttpNotFound();
            }
            return View(sss);
        }

        // POST: Admin/SSS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblSSS model)
        {
            if (ModelState.IsValid)
            {
                var sss = db.TblSSS.Find(model.Id);
                if (sss != null)
                {
                    sss.Soru = model.Soru;
                    sss.Cevap = model.Cevap;
                    sss.Kategori = model.Kategori;
                    sss.Sira = model.Sira;
                    sss.AktifMi = model.AktifMi;
                    db.SaveChanges();
                    TempData["Success"] = "Soru güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // POST: Admin/SSS/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var sss = db.TblSSS.Find(id);
            if (sss != null)
            {
                db.TblSSS.Remove(sss);
                db.SaveChanges();
                TempData["Success"] = "Soru silindi.";
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
