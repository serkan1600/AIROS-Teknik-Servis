using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using System;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class NewsletterController : Controller
    {
        private AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Newsletter
        public ActionResult Index()
        {
            var unread = db.TblNewsletter.Where(x => !x.Okundu).ToList();
            if (unread.Any())
            {
                foreach (var item in unread)
                {
                    item.Okundu = true;
                }
                db.SaveChanges();
            }

            var subscribers = db.TblNewsletter.OrderByDescending(x => x.KayitTarihi).ToList();
            return View(subscribers);
        }

        // POST: Admin/Newsletter/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            var sub = db.TblNewsletter.Find(id);
            if (sub != null)
            {
                sub.AktifMi = !sub.AktifMi;
                db.SaveChanges();
                return Json(new { success = true, status = sub.AktifMi });
            }
            return Json(new { success = false });
        }

        // POST: Admin/Newsletter/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var sub = db.TblNewsletter.Find(id);
            if (sub != null)
            {
                db.TblNewsletter.Remove(sub);
                db.SaveChanges();
                TempData["Success"] = "Abone silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}
