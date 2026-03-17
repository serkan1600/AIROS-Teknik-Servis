using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class SeoFixController : Controller
    {
        private AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RunFix()
        {
            int stats = 0;

            // 0. KRİTİK: Eksik Sütunları Tamamla (SQL ile Doğrudan Müdahale)
            try
            {
                db.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblSeo' AND COLUMN_NAME = 'Keywords') BEGIN ALTER TABLE TblSeo ADD Keywords nvarchar(max) NULL END");
                db.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblAyarlar' AND COLUMN_NAME = 'StickyBarAktif') BEGIN ALTER TABLE TblAyarlar ADD StickyBarAktif bit NULL END");
                db.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblAyarlar' AND COLUMN_NAME = 'StickyBarMetin') BEGIN ALTER TABLE TblAyarlar ADD StickyBarMetin nvarchar(max) NULL END");
                db.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblAyarlar' AND COLUMN_NAME = 'GoogleMapsYorumId') BEGIN ALTER TABLE TblAyarlar ADD GoogleMapsYorumId nvarchar(max) NULL END");
                db.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblAyarlar' AND COLUMN_NAME = 'ExitPopupAktif') BEGIN ALTER TABLE TblAyarlar ADD ExitPopupAktif bit NULL END");
                db.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TblAyarlar' AND COLUMN_NAME = 'ExitPopupTekrarSuresi') BEGIN ALTER TABLE TblAyarlar ADD ExitPopupTekrarSuresi int NULL END");
            }
            catch { }

            // 1. Slider (Hero) Güncelleme
            var heroSlider = db.TblIcerikler.FirstOrDefault(x => x.BolumAdi == "Hero" || x.BolumAdi == "Slider");
            if (heroSlider != null)
            {
                heroSlider.Baslik = "Bursa Kombi ve Klima Teknik Servisi";
                heroSlider.Aciklama = "Bursa genelinde 7/24 profesyonel kombi bakımı, tamiri ve klima servis hizmetleri. Ekonomik fiyatlar ve garantili işçilik!";
                stats++;
            }

            // 2. Kombi Hizmeti 
            var kombiHizmet = db.TblHizmetler.ToList().FirstOrDefault(x => x.Baslik.Contains("Kombi"));
            if (kombiHizmet != null)
            {
                kombiHizmet.Baslik = "Bursa Kombi Servisi (Bakım ve Tamir)";
                kombiHizmet.Aciklama = "Bursa'da uzman kadromuzla her marka kombi için yıllık bakım, arıza tespiti ve garantili tamir hizmeti sunuyoruz.";
                stats++;
            }

            // 3. Ana Sayfa SEO 
            var anasayfaSeo = db.TblSeo.FirstOrDefault(x => x.SayfaAdi == "Home" || x.SayfaAdi == "Anasayfa");
            if (anasayfaSeo != null)
            {
                anasayfaSeo.Title = "Bursa Kombi ve Klima Servisi | 7/24 Teknik Servis";
                anasayfaSeo.Description = "Bursa genelinde profesyonel kombi bakımı, tamiri ve klima teknik servis hizmeti. Hızlı çözüm ve 1 yıl servis garantisi.";
                anasayfaSeo.Keywords = "bursa kombi servisi, bursa kombi bakımı, bursa kombi tamiri, bursa klima servisi";
                stats++;
            }

            db.SaveChanges();
            TempData["Success"] = $"SEO onarımı tamamlandı! {stats} alan güncellendi.";
            return RedirectToAction("Index");
        }
    }
}
