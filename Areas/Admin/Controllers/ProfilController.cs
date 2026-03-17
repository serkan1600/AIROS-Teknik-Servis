using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AIROSWEB.DAL.Db;
using AIROSWEB.BLL.Managers;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class ProfilController : Controller
    {
        AdminManager adminManager = new AdminManager();

        public ActionResult Index()
        {
            string username = User.Identity.Name;
            var admin = adminManager.BasariliGiris(username);
            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Guncelle(TblAdmin p, string yeniSifre)
        {
            var admin = adminManager.BasariliGiris(User.Identity.Name);
            if (admin != null)
            {
                admin.Email = p.Email;
                admin.GizliSoru = p.GizliSoru;
                admin.GizliCevap = p.GizliCevap;

                if (!string.IsNullOrEmpty(yeniSifre))
                {
                    admin.Sifre = HashHelper.ComputeSha256Hash(yeniSifre);
                }

                adminManager.AdminGuncelle(admin);
                TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
            }
            return RedirectToAction("Index");
        }
    }
}
