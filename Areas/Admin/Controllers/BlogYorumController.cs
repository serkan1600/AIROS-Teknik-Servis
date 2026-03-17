using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class BlogYorumController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            // Include kullanımı için System.Data.Entity gerekir.
            var values = db.TblBlogYorum.Include(x => x.Blog).OrderByDescending(x => x.Id).ToList();
            return View(values);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Onayla(int id)
        {
            var value = db.TblBlogYorum.Find(id);
            if(value != null)
            {
                value.OnayliMi = !value.OnayliMi;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var value = db.TblBlogYorum.Find(id);
            if(value != null)
            {
                db.TblBlogYorum.Remove(value);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Cevapla(int id)
        {
            var value = db.TblBlogYorum.Include(x => x.Blog).FirstOrDefault(x => x.Id == id);
            if (value == null) return HttpNotFound();
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Cevapla(TblBlogYorum p)
        {
            var value = db.TblBlogYorum.Find(p.Id);
            if (value != null)
            {
                value.YoneticiCevabi = p.YoneticiCevabi;
                value.CevapTarihi = DateTime.Now;
                value.OnayliMi = true; // Cevaplayınca otomatik onayla
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
