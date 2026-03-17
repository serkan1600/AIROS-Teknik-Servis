using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            // Genel İstatistikler
            ViewBag.ToplamHizmet = db.TblHizmetler.Count();
            ViewBag.AktifHizmet = db.TblHizmetler.Count(x => x.AktifMi == true);
            
            ViewBag.ToplamMarka = db.TblMarkalar.Count();
            ViewBag.AktifMarka = db.TblMarkalar.Count(x => x.AktifMi == true);
            
            ViewBag.ToplamIcerik = db.TblIcerikler.Count();
            ViewBag.AktifIcerik = db.TblIcerikler.Count(x => x.AktifMi);
            
            ViewBag.ToplamSayfa = db.TblSayfalar.Count();
            ViewBag.AktifSayfa = db.TblSayfalar.Count(x => x.AktifMi);

            // Mesaj İstatistikleri
            ViewBag.ToplamMesaj = db.TblMesajlar.Count();
            ViewBag.OkunmamisMesaj = db.TblMesajlar.Count(x => !x.Okundu);
            ViewBag.YanitlanmamisMesaj = db.TblMesajlar.Count(x => !x.Yanitlandi);
            ViewBag.BugunkuMesaj = db.TblMesajlar.Count(x => x.Tarih >= DateTime.Today);

            // Galeri İstatistikleri (varsa)
            ViewBag.ToplamGaleri = db.TblGaleri.Count();
            ViewBag.AktifGaleri = db.TblGaleri.Count(x => x.AktifMi);

            // Blog İstatistikleri (varsa)
            ViewBag.ToplamBlog = db.TblBlog.Count();
            ViewBag.AktifBlog = db.TblBlog.Count(x => x.AktifMi);
            ViewBag.ToplamBlogKategori = db.TblBlogKategori.Count();

            // SSS İstatistikleri (varsa)
            ViewBag.ToplamSSS = db.TblSSS.Count();
            ViewBag.AktifSSS = db.TblSSS.Count(x => x.AktifMi);

            // Randevu İstatistikleri
    ViewBag.ToplamRandevu = db.TblRandevular.Count();
    ViewBag.BekleyenRandevu = db.TblRandevular.Count(x => x.Durum == "Beklemede");
    ViewBag.BugunkuRandevu = db.TblRandevular.Count(x => x.RandevuTarihi >= DateTime.Today && x.RandevuTarihi < DbFunctions.AddDays(DateTime.Today, 1));

            // Son Mesajlar (5 adet)
            ViewBag.SonMesajlar = db.TblMesajlar.AsNoTracking()
                .OrderByDescending(x => x.Tarih)
                .Take(5)
                .ToList();
                
            // Bekleyen Yorumlar
            ViewBag.BekleyenYorumSayisi = db.TblMusteriYorumlari.Count(x => x.AktifMi == false);
            ViewBag.BekleyenYorumlar = db.TblMusteriYorumlari.AsNoTracking()
                .Where(x => x.AktifMi == false)
                .OrderByDescending(x => x.Tarih)
                .Take(5)
                .ToList();

            // --- GRAFİK VERİLERİ ---

            // 1. Haftalık Randevu Grafiği (Son 7 Gün)
            var son7Gun = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-6 + i)).ToList();
            var randevuData = new int[7];
            var randevuLabels = new string[7];

            for (int i = 0; i < 7; i++)
            {
                var tarih = son7Gun[i];
                var sonrakiGun = tarih.AddDays(1);
                // DbFunctions.TruncateTime kullanılabilir veya tarih aralığı
                randevuData[i] = db.TblRandevular.Count(x => x.RandevuTarihi >= tarih && x.RandevuTarihi < sonrakiGun);
                randevuLabels[i] = tarih.ToString("dd MMM");
            }
            ViewBag.HaftalikRandevuData = randevuData;
            ViewBag.HaftalikRandevuLabels = randevuLabels;

            // 1.5 Aylık Servis Talepleri (Son 6 Ay)
            var son6Ay = new System.Collections.Generic.List<DateTime>();
            for(int i = 5; i >= 0; i--)
            {
                son6Ay.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-i));
            }
            var aylikData = new int[6];
            var aylikLabels = new string[6];
            for (int i = 0; i < 6; i++)
            {
                var baslangic = son6Ay[i];
                var bitis = baslangic.AddMonths(1);
                aylikData[i] = db.TblRandevular.Count(x => x.RandevuTarihi >= baslangic && x.RandevuTarihi < bitis);
                aylikLabels[i] = baslangic.ToString("MMM yyyy");
            }
            ViewBag.AylikRandevuData = aylikData;
            ViewBag.AylikRandevuLabels = aylikLabels;


            // 2. Hizmet Popülerliği (Randevulara Göre)
            var hizmetler = db.TblRandevular.AsNoTracking()
                .GroupBy(x => x.HizmetAdi)
                .Select(g => new { Hizmet = g.Key, Sayi = g.Count() })
                .OrderByDescending(x => x.Sayi)
                .Take(5)
                .ToList();
            
            ViewBag.HizmetLabels = hizmetler.Select(x => string.IsNullOrEmpty(x.Hizmet) ? "Diğer" : x.Hizmet).ToList();
            ViewBag.HizmetData = hizmetler.Select(x => x.Sayi).ToList();

            // 3. Blog Kategori Dağılımı
            var blogKategoriler = db.TblBlog.AsNoTracking()
                .GroupBy(x => x.KategoriId)
                .Select(g => new { KategoriId = g.Key, Sayi = g.Count() })
                .ToList();

            // Kategori isimlerini çekmek için (Client-side join yerine burada yapalım)
            var kategoriIsimleri = db.TblBlogKategori.AsNoTracking().ToList();
            var blogLabels = new System.Collections.Generic.List<string>();
            var blogData = new System.Collections.Generic.List<int>();

            foreach(var item in blogKategoriler)
            {
                var kat = kategoriIsimleri.FirstOrDefault(x => x.Id == item.KategoriId);
                blogLabels.Add(kat?.KategoriAdi ?? "Genel");
                blogData.Add(item.Sayi);
            }

            ViewBag.BlogLabels = blogLabels;
            ViewBag.BlogData = blogData;

            return View();
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
