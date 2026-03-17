using System;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Admin
        public ActionResult Index()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var adminler = db.TblAdmin.ToList();
            return View(adminler);
        }

        // GET: Admin/Admin/Create
        public ActionResult Create()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        // POST: Admin/Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblAdmin model)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
                return RedirectToAction("Index", "Dashboard");

            if (ModelState.IsValid)
            {
                // Check if username exists
                if (db.TblAdmin.Any(x => x.KullaniciAdi == model.KullaniciAdi))
                {
                    ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor!");
                    return View(model);
                }

                // Sifreyi hashle
                model.Sifre = HashHelper.ComputeSha256Hash(model.Sifre);
                model.AktifMi = true; // varsayılan
                
                db.TblAdmin.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Yeni personel/yönetici eklendi.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/Admin/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var admin = db.TblAdmin.Find(id);
            if (admin == null) return HttpNotFound();

            return View(admin);
        }

        // POST: Admin/Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TblAdmin model)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var mevcut = db.TblAdmin.Find(model.Id);
            if (mevcut == null) return HttpNotFound();

            // Username uniq check if changed
            if (mevcut.KullaniciAdi != model.KullaniciAdi && db.TblAdmin.Any(x => x.KullaniciAdi == model.KullaniciAdi))
            {
                ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor!");
                return View(model);
            }

            mevcut.KullaniciAdi = model.KullaniciAdi;
            mevcut.Email = model.Email;
            mevcut.Rol = model.Rol;
            mevcut.AktifMi = model.AktifMi;

            if (!string.IsNullOrEmpty(model.Sifre))
            {
                mevcut.Sifre = HashHelper.ComputeSha256Hash(model.Sifre);
            }

            db.SaveChanges();
            TempData["Success"] = "Kullanıcı güncellendi.";
            return RedirectToAction("Index");
        }

        // POST: Admin/Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var admin = db.TblAdmin.Find(id);
            if (admin != null)
            {
                var suanKiKullanici = Session["AdminId"]?.ToString();
                if (suanKiKullanici == admin.Id.ToString())
                {
                    TempData["Error"] = "Kendinizi silemezsiniz!";
                }
                else
                {
                    db.TblAdmin.Remove(admin);
                    db.SaveChanges();
                    TempData["Success"] = "Kullanıcı silindi.";
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
