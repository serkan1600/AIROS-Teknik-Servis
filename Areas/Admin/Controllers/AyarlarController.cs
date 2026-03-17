using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class AyarlarController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Ayarlar
        public ActionResult Index()
        {
            var model = db.TblAyarlar.FirstOrDefault();
            if (model == null)
            {
                // Eğer ayar yoksa varsayılan oluştur
                model = new TblAyarlar();
                db.TblAyarlar.Add(model);
                db.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(TblAyarlar model)
        {
            if (ModelState.IsValid)
            {
                var ayar = db.TblAyarlar.FirstOrDefault();
                if (ayar != null)
                {
                    // Temel Bilgiler
                    ayar.SiteBaslik = model.SiteBaslik;
                    ayar.LogoYolu = model.LogoYolu;
                    ayar.FooterLogoYolu = model.FooterLogoYolu;
                    ayar.FaviconYolu = model.FaviconYolu;
                    
                    // İletişim Bilgileri
                    ayar.Telefon = model.Telefon;
                    ayar.Telefon2 = model.Telefon2;
                    ayar.WhatsApp = model.WhatsApp;
                    ayar.Email = model.Email;
                    ayar.Adres = model.Adres;
                    ayar.HaritaEmbed = model.HaritaEmbed;
                    
                    // Sosyal Medya
                    ayar.Facebook = model.Facebook;
                    ayar.Instagram = model.Instagram;
                    ayar.Twitter = model.Twitter;
                    ayar.LinkedIn = model.LinkedIn;
                    ayar.YouTube = model.YouTube;
                    
                    // Diğer
                    ayar.FooterAciklama = model.FooterAciklama;
                    ayar.CalismaGunleri = model.CalismaGunleri;
                    ayar.CalismaSaatleri = model.CalismaSaatleri;
                    ayar.GoogleAnalyticsId = model.GoogleAnalyticsId;
                    ayar.GoogleVerificationCode = model.GoogleVerificationCode;
                    ayar.PwaThemeColor = model.PwaThemeColor;
                    ayar.PwaAppShortName = model.PwaAppShortName;
                    
                    // Yeni Gelişmiş Özellikler
                    ayar.StickyBarAktif = model.StickyBarAktif;
                    ayar.StickyBarMetin = model.StickyBarMetin;
                    ayar.StickyBarBaslangic = model.StickyBarBaslangic;
                    ayar.StickyBarBitis = model.StickyBarBitis;
                    ayar.GoogleMapsYorumId = model.GoogleMapsYorumId;
                    ayar.ExitPopupAktif = model.ExitPopupAktif;
                    ayar.ExitPopupGecikme = model.ExitPopupGecikme;
                    ayar.ExitPopupTekrarSuresi = model.ExitPopupTekrarSuresi;

                    // Sosyal Kanıt Ayarları
                    ayar.SosyalKanitGecikme = model.SosyalKanitGecikme;
                    ayar.SosyalKanitTekrar = model.SosyalKanitTekrar;

                    // --- SONRADAN EKLENEN ÖZELLİKLER ---
                    ayar.BakimModu = model.BakimModu;
                    ayar.BakimMesaji = model.BakimMesaji;
                    ayar.WhatsAppMesaj = model.WhatsAppMesaj;
                    ayar.HeaderKod = model.HeaderKod;
                    ayar.FooterKod = model.FooterKod;
                    ayar.InstagramToken = model.InstagramToken;
                    ayar.NewsletterAktif = model.NewsletterAktif;
                    
                    db.SaveChanges();
                    CacheHelper.ClearLayoutCache();
                    LogHelper.LogAudit("Güncelleme", "TblAyarlar", ayar.Id, "Genel site ayarları güncellendi.");
                    TempData["Success"] = "Ayarlar başarıyla güncellendi.";
                }
            }
            return View(model);
        }

        // GET: Admin/Ayarlar/Popup
        public ActionResult Popup()
        {
            var model = db.TblAyarlar.FirstOrDefault();
            if (model == null)
            {
                model = new TblAyarlar();
                db.TblAyarlar.Add(model);
                db.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Popup(TblAyarlar model)
        {
            var ayar = db.TblAyarlar.FirstOrDefault();
            if (ayar == null)
            {
                ayar = new TblAyarlar();
                db.TblAyarlar.Add(ayar);
            }
            
            // Sadece Popup ile ilgili alanları dönüştürdüğümüz için
            // diğer değerlerin zorunlu alan kontrolüne (ModelState) takılmaması için doğrudan kaydediyoruz.
            ayar.PopupAktif = model.PopupAktif;
            ayar.PopupBaslik = model.PopupBaslik;
            ayar.PopupIcerik = model.PopupIcerik;
            ayar.PopupGecikme = model.PopupGecikme;
            ayar.PopupTekrarSuresi = model.PopupTekrarSuresi;
            
            db.SaveChanges();
            CacheHelper.ClearLayoutCache();
            TempData["Success"] = "Popup kampanya ayarları güncellendi.";
            
            return View(ayar);
        }
    }
}

