using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class LogsController : Controller
    {
        private AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Logs/Audit
        public ActionResult Audit()
        {
            var logs = db.TblAuditLogs.OrderByDescending(x => x.Tarih).Take(500).ToList();
            return View(logs);
        }

        // GET: Admin/Logs/Security
        public ActionResult Security()
        {
            var logs = db.TblSecurityLogs.OrderByDescending(x => x.Tarih).Take(500).ToList();
            return View(logs);
        }

        // POST: Admin/Logs/ClearAudit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearAudit()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE TblAuditLogs");
            TempData["Success"] = "İşlem günlükleri temizlendi.";
            return RedirectToAction("Audit");
        }

        // POST: Admin/Logs/ClearSecurity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearSecurity()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE TblSecurityLogs");
            TempData["Success"] = "Güvenlik günlükleri temizlendi.";
            return RedirectToAction("Security");
        }
    }
}
