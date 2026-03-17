using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using PagedList;
using AIROSWEB.DAL.Db;
using AIROSWEB.Models;
using AIROSWEB.Helpers;

namespace AIROSWEB.Controllers
{
    public class HomeController : BaseController
    {



        public ActionResult Index()
        {
            // PROAKTİF MARKA GÜNCELLEME: Sadece 24 saatte bir veya uygulama başladığında kontrol et
            string brandCheckKey = "BrandUpdateDone";
            if (HttpRuntime.Cache[brandCheckKey] == null)
            {
                try
                {
                    string[] targetBrands = { "Bosch", "Vaillant", "Demirdöküm", "Baymak", "ECA", "Viessmann", "Buderus", "Ariston", "Copa" };
                    var existingInDb = db.TblMarkalar.ToList();
                    bool isChanged = false;

                    foreach (var brandName in targetBrands)
                    {
                        var found = existingInDb.FirstOrDefault(x => x.MarkaAdi.ToLower().Contains(brandName.ToLower()));
                        string fileName = brandName.ToLower().Replace("ü", "u").Replace("ö", "o");
                        string path = $"/Uploads/Markalar/{fileName}.png";

                        if (found == null)
                        {
                            db.TblMarkalar.Add(new TblMarkalar { MarkaAdi = brandName, AktifMi = true, LogoYolu = path });
                            isChanged = true;
                        }
                        else if (found.AktifMi == false || string.IsNullOrEmpty(found.LogoYolu))
                        {
                            found.AktifMi = true;
                            found.LogoYolu = path;
                            isChanged = true;
                        }
                    }

                    if (isChanged) db.SaveChanges();
                    
                    // 24 saat boyunca tekrar kontrol etme
                    HttpRuntime.Cache.Insert(brandCheckKey, true, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                catch { /* Hata olsa bile ana sayfa açılmaya devam etsin */ }
            }

            var model = GetBaseModel("Anasayfa");
            model.Hizmetler = db.TblHizmetler.AsNoTracking().Where(x => x.AktifMi == true).ToList();
            model.Markalar = db.TblMarkalar.AsNoTracking().Where(x => x.AktifMi == true).ToList();
            model.Galeri = db.TblGaleri.AsNoTracking().Where(x => x.AktifMi == true).OrderBy(x => x.Sira).Take(8).ToList();
            model.Yorumlar = db.TblMusteriYorumlari.AsNoTracking().Where(x => x.AktifMi == true).OrderByDescending(x => x.Id).Take(6).ToList();
            
            // SEO: Eğer DB'den özel title gelmediyse anahtar kelime ağırlıklı title kullan
            if (model.Seo == null || string.IsNullOrEmpty(model.Seo.Title))
            {
                ViewBag.Title = "Bursa Kombi ve Klima Servisi | 7/24 Teknik Servis";
            }
            if (model.Seo == null || string.IsNullOrEmpty(model.Seo.Description))
            {
                ViewBag.Description = "Bursa genelinde profesyonel kombi bakımı, kombi tamiri, klima servisi ve beyaz eşya teknik servis hizmeti. Uygun fiyat ve orijinal yedek parça garantisi.";
            }

            return View(model);
        }

        public ActionResult Hakkimizda()
        {
            var model = GetBaseModel("Hakkimizda");
            return View(model);
        }

        public ActionResult Hizmetler(int? id, string seoTitle = "")
        {
            if (id.HasValue && id > 0)
            {
                var service = db.TblHizmetler.AsNoTracking().FirstOrDefault(x => x.Id == id && x.AktifMi == true);
                if (service != null)
                {
                    ViewBag.Title = service.MetaTitle ?? service.Baslik + " | Bursa Teknik Servis";
                    ViewBag.Description = service.MetaDescription ?? service.Aciklama ?? (service.Baslik + " hizmetimiz hakkında detaylı bilgi, fiyatlar ve servis randevusu için tıklayın.");
                    
                    // Dinamik Open Graph Görseli
                    if (!string.IsNullOrEmpty(service.ResimYolu)) { ViewBag.OgImage = service.ResimYolu; }
                    
                    // Detay sayfasının layout modelini hazırla
                    var detailModel = GetBaseModel("Hizmetlerimiz");
                    ViewBag.CurrentService = service;
                    return View("HizmetDetay", detailModel);
                }
            }

            var model = GetBaseModel("Hizmetlerimiz");
            model.Hizmetler = db.TblHizmetler.AsNoTracking().Where(x => x.AktifMi == true).ToList();
            return View(model);
        }

        public ActionResult Iletisim()
        {
            var model = GetBaseModel("Iletisim");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Iletisim(TblMesajlar model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Tarih = DateTime.Now;
                    model.Okundu = false;
                    model.Yanitlandi = false;

                    db.TblMesajlar.Add(model);
                    db.SaveChanges();

                    TempData["Success"] = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.";
                    return RedirectToAction("Iletisim");
                }
                
                TempData["Error"] = "Lütfen formdaki tüm alanları doğru doldurduğunuzdan emin olun.";
                return RedirectToAction("Iletisim");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Mesaj gönderilirken bir hata oluştu: " + ex.Message;
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = TempData["Error"] == null, message = TempData["Success"] ?? TempData["Error"] });
            }

            return RedirectToAction("Iletisim");
        }
        public ActionResult FiyatHesapla()
        {
            var model = GetBaseModel("Fiyat Hesapla");
            ViewBag.Title = "Fiyat Hesaplatma Sihirbazı | AIROS";
            ViewBag.Description = "Cihazınızın markası, türü ve şikayetini seçerek anında tahmini servis fiyatı alın.";
            
            var arizalar = db.TblArizaFiyatlari.Where(x => x.AktifMi).OrderBy(x => x.Sira).Select(x => new {
                x.CihazTuru,
                Marka = string.IsNullOrEmpty(x.Marka) ? "Tümü" : x.Marka,
                x.ArizaAdi,
                x.FiyatAraligi
            }).ToList();
            
            ViewBag.ArizalarJson = System.Web.Helpers.Json.Encode(arizalar);
            
            return View(model);
        }

        [HttpGet]
        public ActionResult CihazTakip()
        {
            var model = GetBaseModel("Servis Takip");
            ViewBag.Title = "Canlı Servis Takip | AIROS";
            ViewBag.Description = "Cihazınızın servis durumunu telefon numaranız ile anlık olarak takip edin.";
            return View(model);
        }

        [HttpPost]
        public ActionResult CihazTakip(string telefonNo)
        {
            var model = GetBaseModel("Servis Takip");
            ViewBag.Title = "Canlı Servis Takip | AIROS";

            if (string.IsNullOrEmpty(telefonNo))
            {
                ViewBag.Hata = "Lütfen telefon numaranızı girin.";
                return View(model);
            }

            // Remove non-numeric
            var cleanPhone = new string(telefonNo.Where(char.IsDigit).ToArray());
            if (cleanPhone.StartsWith("0")) cleanPhone = cleanPhone.Substring(1);
            if (cleanPhone.StartsWith("90") && cleanPhone.Length == 12) cleanPhone = cleanPhone.Substring(2);

            var randevular = db.TblRandevular
                .Where(x => x.Telefon != null && x.Telefon.Contains(cleanPhone))
                .OrderByDescending(x => x.OlusturmaTarihi)
                .ToList();

            ViewBag.TelefonNumarasi = telefonNo;
            ViewBag.Randevular = randevular;

            if (!randevular.Any())
            {
                ViewBag.Hata = $"'{telefonNo}' numarasına ait aktif bir servis kaydı veya randevu bulunamadı.";
            }

            return View(model);
        }

        public ActionResult Randevu()
        {
            var model = GetBaseModel("Randevu");
            ViewBag.Services = db.TblHizmetler.Where(x => x.AktifMi == true).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult KuponSorgula(string kod)
        {
            try
            {
                if (string.IsNullOrEmpty(kod)) return Json(new { success = false, message = "Kodu boş gönderemezsiniz." }, JsonRequestBehavior.AllowGet);

                var kupon = db.TblKuponlar.FirstOrDefault(x => x.Kod == kod);

                if (kupon == null)
                    return Json(new { success = false, message = "Geçersiz kupon kodu." }, JsonRequestBehavior.AllowGet);

                if (!kupon.AktifMi)
                    return Json(new { success = false, message = "Bu kupon artık aktif değil." }, JsonRequestBehavior.AllowGet);

                if (kupon.SonKullanmaTarihi.HasValue && kupon.SonKullanmaTarihi.Value < DateTime.Now)
                    return Json(new { success = false, message = "Bu kuponun süresi dolmuş." }, JsonRequestBehavior.AllowGet);

                string indirimAciklamasi = kupon.IndirimOrani > 0 ? $"%{kupon.IndirimOrani}" : $"{kupon.Miktar} TL";

                return Json(new { success = true, message = $"Kupon geçerli! ({indirimAciklamasi})" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Randevu(TblRandevular model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.OlusturmaTarihi = DateTime.Now;
                    model.Durum = "Beklemede";

                    // Eğer HizmetId gelmişse adını bulalım
                    if (model.HizmetId.HasValue && model.HizmetId > 0)
                    {
                        var hizmet = db.TblHizmetler.Find(model.HizmetId);
                        if (hizmet != null) model.HizmetAdi = hizmet.Baslik;
                    }
                    else if (model.HizmetId == 0)
                    {
                        model.HizmetAdi = "Diğer";
                    }

                    if (!string.IsNullOrEmpty(model.KuponKodu))
                    {
                        var kupon = db.TblKuponlar.FirstOrDefault(x => x.Kod == model.KuponKodu && x.AktifMi);
                        if (kupon != null && (!kupon.SonKullanmaTarihi.HasValue || kupon.SonKullanmaTarihi.Value >= DateTime.Now))
                        {
                            kupon.KullanimSayisi++;
                        }
                        else
                        {
                            model.KuponKodu = null; // Geçersizse kaydetme
                        }
                    }

                    db.TblRandevular.Add(model);
                    db.SaveChanges();

                    TempData["Success"] = "Randevu talebiniz başarıyla alındı. En kısa sürede sizinle iletişime geçeceğiz.";
                    return RedirectToAction("Randevu");
                }
                
                TempData["Error"] = "Lütfen tüm zorunlu alanları doldurun.";
                return RedirectToAction("Randevu");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşlem sırasında bir hata oluştu: " + ex.Message;
                return RedirectToAction("Randevu");
            }
        }

        // --- MÜŞTERİ YORUM / PUANLAMA EKRANI ---
        public ActionResult DegerlendirmeYaz()
        {
            var model = GetBaseModel("Değerlendirme");
            ViewBag.Title = "Hizmetimizi Değerlendirin";
            ViewBag.Description = "Aldığınız hizmet hakkında görüşlerinizi paylaşın.";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DegerlendirmeGonder(TblMusteriYorumlari model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Tarih = DateTime.Now;
                    model.AktifMi = false; // Admin onaylamadan yayınlanmayacak
                    
                    if(string.IsNullOrEmpty(model.Meslek)) {
                        model.Meslek = "Müşteri";
                    }

                    db.TblMusteriYorumlari.Add(model);
                    db.SaveChanges();

                    TempData["Success"] = "Değerlendirmeniz başarıyla alındı. Teşekkür ederiz!";
                    return RedirectToAction("DegerlendirmeYaz");
                }
                TempData["Error"] = "Lütfen zorunlu alanları doldurun.";
                return RedirectToAction("DegerlendirmeYaz");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu. " + ex.Message;
                return RedirectToAction("DegerlendirmeYaz");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RandevuAl(string AdSoyad, string Telefon, string HizmetAdi)
        {
            try
            {
                if (!string.IsNullOrEmpty(AdSoyad) && !string.IsNullOrEmpty(Telefon))
                {
                    var randevu = new TblRandevular
                    {
                        AdSoyad = AdSoyad,
                        Telefon = Telefon,
                        HizmetAdi = HizmetAdi,
                        OlusturmaTarihi = DateTime.Now,
                        Durum = "Beklemede",
                        RandevuTarihi = DateTime.Now.Date,
                        Mesaj = "Çıkış Teklif Formundan Gelen Talep"
                    };

                    db.TblRandevular.Add(randevu);
                    db.SaveChanges();

                    TempData["Success"] = "Talebiniz başarıyla alındı. Uzman ekibimiz sizi en kısa sürede arayacaktır.";
                }
                else
                {
                    TempData["Error"] = "Lütfen adınızı ve telefon numaranızı eksiksiz giriniz.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşlem sırasında bir teknik hata oluştu: " + ex.Message;
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = TempData["Error"] == null, message = TempData["Success"] ?? TempData["Error"] });
            }

            return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("Index"));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HizliTeklif(string AdSoyad, string Telefon, string HizmetTuru)
        {
            try
            {
                if (!string.IsNullOrEmpty(AdSoyad) && !string.IsNullOrEmpty(Telefon))
                {
                    var randevu = new TblRandevular
                    {
                        AdSoyad = AdSoyad,
                        Telefon = Telefon,
                        HizmetAdi = HizmetTuru,
                        OlusturmaTarihi = DateTime.Now,
                        Durum = "Hızlı Teklif",
                        RandevuTarihi = DateTime.Now.Date, // Varsayılan bugünün tarihi
                        Mesaj = "Hızlı Teklif Formundan Gelen Talep"
                    };

                    db.TblRandevular.Add(randevu);
                    db.SaveChanges();

                    TempData["Success"] = "Teklif talebiniz alındı! En kısa sürede sizi arayacağız.";
                }
                else
                {
                    TempData["Error"] = "Lütfen adınızı ve telefon numaranızı giriniz.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = TempData["Error"] == null, message = TempData["Success"] ?? TempData["Error"] });
            }

            return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("Index"));
        }

        public ActionResult Galeri(int? sayfa)
        {
            var model = GetBaseModel("Galeri");
            
            int sayfaBoyutu = 12; // Her sayfada 12 resim
            int sayfaNumarasi = (sayfa ?? 1); // Sayfa numarası yoksa 1. sayfa
            
            var galeriListesi = db.TblGaleri
                .Where(x => x.AktifMi)
                .OrderBy(x => x.Sira)
                .ToPagedList(sayfaNumarasi, sayfaBoyutu);
            
            model.Galeri = galeriListesi.ToList();
            ViewBag.GaleriPaged = galeriListesi; // Sayfalama için
            
            return View(model);
        }

        public ActionResult SSS()
        {
            var model = GetBaseModel("SSS");
            model.SSS = db.TblSSS.AsNoTracking().Where(x => x.AktifMi).OrderBy(x => x.Sira).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AboneOl(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "Lütfen geçerli bir e-posta adresi giriniz." });
                }

                var existing = db.TblNewsletter.FirstOrDefault(x => x.Email == email);
                if (existing != null)
                {
                    return Json(new { success = true, message = "Bu e-posta zaten kayıtlı. Teşekkürler!" });
                }

                db.TblNewsletter.Add(new TblNewsletter
                {
                    Email = email,
                    KayitTarihi = DateTime.Now,
                    AktifMi = true,
                    Okundu = false
                });
                db.SaveChanges();

                return Json(new { success = true, message = "Bültenimize başarıyla abone oldunuz!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        // All Service Areas Page
        public ActionResult Bolgeler()
        {
            var model = GetBaseModel("Hizmetler");
            model.HizmetBolgeleri = db.TblHizmetBolgeleri.AsNoTracking().Where(x => x.AktifMi).OrderBy(x => x.Sira).ToList();
            
            ViewBag.Title = "Hizmet Verdiğimiz Yerler";
            ViewBag.Description = "Bursa genelinde tüm ilçe ve semtlere profesyonel kombi, klima ve beyaz eşya teknik servis hizmeti sunuyoruz.";
            
            return View(model);
        }

        // Regional Service Page (Local SEO)
        public ActionResult BolgeHizmet(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return RedirectToAction("Index");

            var model = GetBaseModel("Hizmetler");
            
            // Try by slug first, then try by name fallback
            var bolge = db.TblHizmetBolgeleri.FirstOrDefault(x => x.Slug == slug && x.AktifMi);
            
            if (bolge == null)
            {
                // Fallback: Check if any BolgeAdi matches this slug when converted
                bolge = db.TblHizmetBolgeleri.ToList().FirstOrDefault(x => AIROSWEB.Helpers.SeoHelper.ToSeoUrl(x.BolgeAdi) == slug && x.AktifMi);
            }

            if (bolge == null) return RedirectToAction("SayfaBulunamadi");

            ViewBag.Title = !string.IsNullOrEmpty(bolge.MetaTitle) ? bolge.MetaTitle : $"{bolge.BolgeAdi} Kombi ve Klima Servisi";
            ViewBag.Description = !string.IsNullOrEmpty(bolge.MetaDescription) ? bolge.MetaDescription : $"{bolge.BolgeAdi} bölgesinde profesyonel kombi bakımı, tamiri ve klima teknik servis hizmeti. 7/24 hızlı servis ve orijinal yedek parça garantisi.";
            ViewBag.Keywords = bolge.Keywords;
            
            ViewBag.BolgeAdi = bolge.BolgeAdi;
            ViewBag.BolgeAciklama = bolge.Aciklama;
            ViewBag.BolgeIcerik = bolge.Icerik;

            model.Seo = new TblSeo { 
                Title = ViewBag.Title, 
                Description = ViewBag.Description, 
                Keywords = ViewBag.Keywords 
            };

            return View(model);
        }

        // Test page for social media icons
        public ActionResult SocialTest()
        {
            var model = GetBaseModel("Test");
            return View(model);
        }

        // --- SEO Actions ---

        [Route("sitemap.xml")]
        public ActionResult SitemapXml()
        {
            var sitemapNodes = new List<SitemapNode>();
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            // Statik Sayfalar
            sitemapNodes.Add(new SitemapNode { Url = baseUrl, Priority = 1.0 });
            sitemapNodes.Add(new SitemapNode { Url = baseUrl + "/Home/Hakkimizda", Priority = 0.8 });
            sitemapNodes.Add(new SitemapNode { Url = baseUrl + "/Home/Hizmetler", Priority = 0.9 });
            sitemapNodes.Add(new SitemapNode { Url = baseUrl + "/Home/Galeri", Priority = 0.7 });
            sitemapNodes.Add(new SitemapNode { Url = baseUrl + "/Home/SSS", Priority = 0.6 });
            sitemapNodes.Add(new SitemapNode { Url = baseUrl + "/Home/Iletisim", Priority = 0.8 });
            sitemapNodes.Add(new SitemapNode { Url = baseUrl + "/Home/Randevu", Priority = 0.8 });

            // Dinamik Hizmetler
            var services = db.TblHizmetler.Where(x => x.AktifMi == true).ToList();
            foreach (var service in services)
            {
                sitemapNodes.Add(new SitemapNode 
                { 
                    Url = baseUrl + "/Hizmetler/" + service.Id + "/" + AIROSWEB.Helpers.SeoHelper.ToSeoUrl(service.Baslik), 
                    Priority = 0.9,
                    ChangeFrequency = "weekly"
                });
            }

            // Dinamik Blog Yazıları
            var blogs = db.TblBlog.Where(x => x.AktifMi).OrderByDescending(x => x.YayinTarihi).ToList();
            foreach (var blog in blogs)
            {
                sitemapNodes.Add(new SitemapNode
                {
                    Url = baseUrl + "/Blog/Detay/" + blog.Slug,
                    Priority = 0.7,
                    LastModified = blog.YayinTarihi
                });
            }

            // Dinamik Hizmet Bölgeleri (Local SEO)
            var bolgeler = db.TblHizmetBolgeleri.Where(x => x.AktifMi).ToList();
            foreach (var bolge in bolgeler)
            {
                sitemapNodes.Add(new SitemapNode
                {
                    Url = baseUrl + "/Bursa-Teknik-Servis/" + (bolge.Slug ?? AIROSWEB.Helpers.SeoHelper.ToSeoUrl(bolge.BolgeAdi)),
                    Priority = 0.8,
                    ChangeFrequency = "monthly"
                });
            }

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";

            foreach (var node in sitemapNodes)
            {
                xml += "<url>";
                xml += $"<loc>{node.Url}</loc>";
                xml += $"<lastmod>{(node.LastModified ?? DateTime.Now).ToString("yyyy-MM-dd")}</lastmod>";
                xml += $"<changefreq>{node.ChangeFrequency ?? "monthly"}</changefreq>";
                xml += $"<priority>{node.Priority.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)}</priority>";
                xml += "</url>";
            }

            xml += "</urlset>";

            return Content(xml, "text/xml");
        }

        [Route("robots.txt")]
        public ActionResult RobotsTxt()
        {
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string content = "User-agent: *\n";
            content += "Disallow: /Admin/\n";
            content += "Disallow: /Account/\n";
            content += $"Sitemap: {baseUrl}/sitemap.xml";

            return Content(content, "text/plain");
        }

        [Route("manifest.json")]
        public ActionResult Manifest()
        {
            var ayar = db.TblAyarlar.FirstOrDefault();
            var manifest = new
            {
                name = ayar?.SiteBaslik ?? "AIROS Bursa Teknik Servis",
                short_name = ayar?.PwaAppShortName ?? "AIROS",
                description = ayar?.FooterAciklama ?? "Bursa kombi, klima ve beyaz eşya teknik servisi.",
                start_url = "/",
                display = "standalone",
                background_color = "#ffffff",
                theme_color = ayar?.PwaThemeColor ?? "#0d6efd",
                icons = new[]
                {
                    new { src = string.IsNullOrEmpty(ayar?.LogoYolu) ? Url.Content("~/Content/Images/logo.png") : Url.Content(ayar.LogoYolu), sizes = "192x192", type = "image/png" },
                    new { src = string.IsNullOrEmpty(ayar?.LogoYolu) ? Url.Content("~/Content/Images/logo.png") : Url.Content(ayar.LogoYolu), sizes = "512x512", type = "image/png" }
                }
            };

            return Json(manifest, JsonRequestBehavior.AllowGet);
        }

        // --- HATA SAYFALARI ---
        [Route("SayfaBulunamadi")]
        public ActionResult SayfaBulunamadi()
        {
            Response.StatusCode = 404;
            ViewBag.Title = "Sayfa Bulunamadı - 404";
            ViewBag.ErrorTitle = "Aradığınız Sayfayı Bulamadık";
            ViewBag.ErrorMessage = "Gitmek istediğiniz sayfa silinmiş, taşınmış veya adresi yanlış olabilir.";
            return View("ErrorPage"); // Ortak hata view'ı kullanalım
        }

        [Route("SistemHatasi")]
        public ActionResult SistemHatasi()
        {
            Response.StatusCode = 500;
            ViewBag.Title = "Sistem Hatası - 500";
            ViewBag.ErrorTitle = "Bir Hata Oluştu";
            ViewBag.ErrorMessage = "Sunucularımızda beklenmedik bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
            return View("ErrorPage");
        }

        public class SitemapNode
        {
            public string Url { get; set; }
            public double Priority { get; set; }
            public string ChangeFrequency { get; set; }
            public DateTime? LastModified { get; set; }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            var sayfa = db.TblSayfalar.FirstOrDefault(x => x.Link.Replace("/", "") == actionName || x.Link.Replace("/Home/", "") == actionName || x.Link.ToLower() == "/home/" + actionName.ToLower() || x.Link.ToLower() == "/" + actionName.ToLower());
            
            if (sayfa != null && sayfa.AktifMi)
            {
                ViewBag.Sayfa = sayfa;
                var model = GetBaseModel("Sayfa_" + sayfa.Baslik);
                this.View("DinamikSayfa", model).ExecuteResult(this.ControllerContext);
            }
            else
            {
                // Eğer sayfa Pasife çekilmişse veya hiç yoksa otomatik 404 sayfasına (Müşteri Dostu) gönder
                Response.Redirect("/SayfaBulunamadi");
            }
        }
    }
}
