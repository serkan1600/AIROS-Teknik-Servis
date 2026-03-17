using System.Web.Mvc;
using System.Web.Security;
using AIROSWEB.BLL.Managers;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        AdminManager adminManager = new AdminManager();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string kullaniciAdi, string sifre)
        {
            string hashliSifre = HashHelper.ComputeSha256Hash(sifre);
            var admin = adminManager.AdminGiris(kullaniciAdi, hashliSifre);

            if (admin != null)
            {
                LogHelper.LogSecurity(kullaniciAdi, "Giriş", "Başarılı");
                FormsAuthentication.SetAuthCookie(admin.KullaniciAdi, false);
                Session["AdminId"] = admin.Id;
                Session["Rol"] = admin.Rol;
                return RedirectToAction("Index", "Dashboard");
            }

            LogHelper.LogSecurity(kullaniciAdi, "Giriş", "Başarısız", "Hatalı şifre veya kullanıcı adı");
            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        public ActionResult Logout()
        {
            LogHelper.LogSecurity(User.Identity.Name, "Çıkış", "Başarılı");
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            // E-posta ile kurtarma su anlik devre disi birakildi (kullanici istegi uzerine)
            return RedirectToAction("SecretQuestion");
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var admin = adminManager.TokenIleBul(token);
            if (admin == null || admin.SifreSifirlamaTokenTarih < System.DateTime.Now)
            {
                TempData["Error"] = "Geçersiz veya süresi dolmuş sıfırlama linki.";
                return RedirectToAction("Index");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string token, string sifre)
        {
            var admin = adminManager.TokenIleBul(token);
            if (admin == null || admin.SifreSifirlamaTokenTarih < System.DateTime.Now)
            {
                TempData["Error"] = "Geçersiz veya süresi dolmuş sıfırlama linki.";
                return RedirectToAction("Index");
            }

            admin.Sifre = HashHelper.ComputeSha256Hash(sifre);
            admin.SifreSifirlamaToken = null;
            admin.SifreSifirlamaTokenTarih = null;
            adminManager.AdminGuncelle(admin);

            TempData["Success"] = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SecretQuestion()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecretQuestion(string kullaniciAdi, string cevap, string yeniSifre)
        {
            var admin = adminManager.BasariliGiris(kullaniciAdi);
            if (admin != null)
            {
                // Cevap kontrolü (Büyük/Küçük harf duyarsızlığı için ToLower kullanılabilir)
                if (admin.GizliCevap != null && admin.GizliCevap.Trim().ToLower() == cevap.Trim().ToLower())
                {
                    admin.Sifre = HashHelper.ComputeSha256Hash(yeniSifre);
                    adminManager.AdminGuncelle(admin);
                    TempData["Success"] = "Şifreniz gizli soru doğrulamasıyla başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Hata = "Gizli soru cevabı hatalı!";
                    ViewBag.KullaniciAdi = kullaniciAdi;
                    ViewBag.Soru = admin.GizliSoru;
                }
            }
            else
            {
                ViewBag.Hata = "Kullanıcı bulunamadı.";
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetSecretQuestion(string kullaniciAdi)
        {
            var admin = adminManager.BasariliGiris(kullaniciAdi);
            if (admin != null && !string.IsNullOrEmpty(admin.GizliSoru))
            {
                return Json(new { success = true, soru = admin.GizliSoru });
            }
            return Json(new { success = false, message = "Bu kullanıcı için gizli soru tanımlanmamış." });
        }
    }
}
