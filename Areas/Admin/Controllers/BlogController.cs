using System;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Blog
        public ActionResult Index()
        {
            var blog = db.TblBlog.OrderByDescending(x => x.YayinTarihi).ToList();
            ViewBag.Kategoriler = db.TblBlogKategori.ToList();
            return View(blog);
        }

        // GET: Admin/Blog/Create
        public ActionResult Create()
        {
            ViewBag.Kategoriler = new SelectList(db.TblBlogKategori.Where(x => x.AktifMi).ToList(), "Id", "KategoriAdi");
            return View();
        }

        // POST: Admin/Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblBlog model)
        {
            ModelState.Remove("YayinTarihi");
            ModelState.Remove("GoruntulemeSayisi");
            
            if (ModelState.IsValid)
            {
                // Slug oluştur
                if (string.IsNullOrEmpty(model.Slug))
                {
                    model.Slug = CreateSlug(model.Baslik);
                }
                
                model.YayinTarihi = DateTime.Now;
                model.GoruntulemeSayisi = 0;
                
                db.TblBlog.Add(model);
                db.SaveChanges();
                LogHelper.LogAudit("Ekleme", "TblBlog", model.Id, $"Yeni blog eklendi: {model.Baslik}");
                TempData["Success"] = "Blog yazısı başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            
            ViewBag.Kategoriler = new SelectList(db.TblBlogKategori.Where(x => x.AktifMi).ToList(), "Id", "KategoriAdi");
            return View(model);
        }

        // GET: Admin/Blog/Edit/5
        public ActionResult Edit(int id)
        {
            var blog = db.TblBlog.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.Kategoriler = new SelectList(db.TblBlogKategori.Where(x => x.AktifMi).ToList(), "Id", "KategoriAdi", blog.KategoriId);
            return View(blog);
        }

        // POST: Admin/Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblBlog model)
        {
            ModelState.Remove("YayinTarihi");
            ModelState.Remove("GoruntulemeSayisi");
            
            if (ModelState.IsValid)
            {
                var blog = db.TblBlog.Find(model.Id);
                if (blog != null)
                {
                    blog.Baslik = model.Baslik;
                    blog.Slug = string.IsNullOrEmpty(model.Slug) ? CreateSlug(model.Baslik) : model.Slug;
                    blog.Ozet = model.Ozet;
                    blog.Icerik = model.Icerik;
                    blog.ResimYolu = model.ResimYolu;
                    blog.KategoriId = model.KategoriId;
                    blog.Yazar = model.Yazar;
                    blog.MetaTitle = model.MetaTitle;
                    blog.MetaDescription = model.MetaDescription;
                    blog.Keywords = model.Keywords;
                    blog.AktifMi = model.AktifMi;
                    
                    db.SaveChanges();
                    LogHelper.LogAudit("Güncelleme", "TblBlog", blog.Id, $"Blog güncellendi: {blog.Baslik}");
                    TempData["Success"] = "Blog yazısı güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            
            ViewBag.Kategoriler = new SelectList(db.TblBlogKategori.Where(x => x.AktifMi).ToList(), "Id", "KategoriAdi", model.KategoriId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SeedContent()
        {
            var seedPosts = new System.Collections.Generic.List<TblBlog>
            {
                new TblBlog
                {
                    Baslik = "Kombi Bakımı Neden Önemlidir? Tasarruf ve Güvenlik İpuçları",
                    Slug = "kombi-bakimi-neden-onemlidir",
                    Ozet = "Kış ayları gelmeden kombi bakımı yaptırmak, doğalgaz faturasında %30'a varan tasarruf sağlar. İşte kombi bakımının püf noktaları...",
                    Icerik = @"<p>Kombi bakımı, hem enerji tasarrufu sağlamak hem de cihazınızın ömrünü uzatmak için yılda en az bir kez yapılması gereken kritik bir işlemdir. Düzenli bakımı yapılmayan kombiler, daha fazla yakıt tüketir ve beklenmedik arızalara yol açabilir.</p>
                               <h3>Kombi Bakımında Neler Yapılır?</h3>
                               <ul>
                                   <li><strong>Fan Temizliği:</strong> Tozlanan fan motoru ses yapar ve verimsiz çalışır.</li>
                                   <li><strong>Eşanjör Kontrolü:</strong> Isı transferini sağlayan parçanın kireçten arındırılması gerekir.</li>
                                   <li><strong>Gaz Ayarı:</strong> Yanma verimini artırmak için gaz basıncı kontrol edilir.</li>
                                   <li><strong>Filtre Temizliği:</strong> Radyatör dönüş filtresinin temizlenmesi ısınma performansını artırır.</li>
                               </ul>
                               <p>Uzman ekibimizle profesyonel kombi bakımı hizmeti almak için hemen bizimle iletişime geçin.</p>",
                    ResimYolu = "https://images.unsplash.com/photo-1507668077129-56e32842fceb?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                    Yazar = "Admin",
                    YayinTarihi = DateTime.Now.AddDays(-5),
                    AktifMi = true,
                    GoruntulemeSayisi = 125,
                    MetaTitle = "Bursa Kombi Bakımı - %30 Tasarruf Garantisi | Airos Teknik",
                    MetaDescription = "Bursa'da profesyonel kombi bakımı hizmeti. Doğalgaz faturanızı düşürün, kombinizin ömrünü uzatın. Hemen randevu alın!",
                    KategoriId = 1
                },
                new TblBlog
                {
                    Baslik = "Klima Neden Soğutmuyor? En Sık Karşılaşılan 5 Arıza",
                    Slug = "klima-neden-sogutmuyor-ariza-cozumleri",
                    Ozet = "Klimanız çalışıyor ama soğutmuyor mu? Gaz eksikliği, filtre tıkanıklığı veya kompresör arızası olabilir. İşte çözümler...",
                    Icerik = @"<p>Sıcak yaz günlerinde klimanızın performans düşüklüğü yaşaması can sıkıcı olabilir. Klimanın soğutmamasının altında yatan en yaygın sebepleri sizler için derledik.</p>
                               <h3>1. Gaz Eksikliği</h3>
                               <p>Klima sistemindeki gaz kaçakları, soğutma performansını doğrudan etkiler. Gaz dolumu mutlaka uzmanlarca yapılmalıdır.</p>
                               <h3>2. Filtre Tıkanıklığı</h3>
                               <p>Hava filtrelerinin tozla dolması hava akışını engeller. Filtrelerinizi ayda bir kez temizlemeyi unutmayın.</p>
                               <h3>3. Kompresör Arızası</h3>
                               <p>Dış ünitedeki motorun çalışmaması durumunda klima sadece fan modunda çalışır gibi hava üfler ama soğutmaz.</p>
                               <p>Klima tamiri ve gaz dolumu işlemleri için 7/24 hizmetinizdeyiz.</p>",
                    ResimYolu = "https://images.unsplash.com/photo-1616788494707-ec28f08d05a1?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                    Yazar = "Teknik Ekip",
                    YayinTarihi = DateTime.Now.AddDays(-12),
                    AktifMi = true,
                    GoruntulemeSayisi = 89,
                    MetaTitle = "Klima Soğutmuyor Ne Yapmalıyım? - Bursa Klima Servisi",
                    MetaDescription = "Klimanız soğutmuyorsa gaz eksikliği veya motor arızası olabilir. Bursa klima servisi olarak tüm marka klimalarda garantili onarım yapıyoruz.",
                    KategoriId = 2
                },
                new TblBlog
                {
                    Baslik = "Petek Temizliği Nasıl Yapılır? Isınma Sorunlarına Kesin Çözüm",
                    Slug = "petek-temizligi-nasil-yapilir",
                    Ozet = "Peteklerin altı soğuk, üstü sıcak mı? Kombi derecesini yükseltmenize rağmen ısınamıyor musunuz? Çözüm makine ile petek temizliği!",
                    Icerik = @"<p>Zamanla radyatörlerin (peteklerin) dibinde biriken tortu, çamur ve kireç; sıcak suyun devir daim yapmasını engeller. Bu durum kombiyi zorlar ve yakıt tüketimini artırır.</p>
                               <h3>Makineli Petek Temizliği Avantajları</h3>
                               <ul>
                                   <li>Petekler sökülmeden, banyo havlupanından makine bağlanarak yapılır.</li>
                                   <li>Özel kimyasallar ile sistemdeki tüm kireç ve tortu çözülür.</li>
                                   <li>Yakıt tasarrufu sağlar ve kombi pompasının ömrünü uzatır.</li>
                                </ul>
                               <p>Evinizi kirletmeden, profesyonel makineli petek temizliği hizmetimizden yararlanmak için kampanyalarımızı inceleyin.</p>",
                    ResimYolu = "https://images.unsplash.com/photo-1545259741-2ea3ebf61fa3?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                    Yazar = "Admin",
                    YayinTarihi = DateTime.Now.AddDays(-20),
                    AktifMi = true,
                    GoruntulemeSayisi = 210,
                    MetaTitle = "Bursa Petek Temizliği - İlaçlı ve Makineli Temizlik",
                    MetaDescription = "Petekleriniz ısınmıyor mu? Bursa'da ilaçlı ve makineli petek temizliği ile %40'a varan ısı artışı garantisi. Hemen arayın!",
                    KategoriId = 5
                }
            };

            int addedCount = 0;
            foreach (var post in seedPosts)
            {
                if (!db.TblBlog.Any(b => b.Slug == post.Slug))
                {
                    db.TblBlog.Add(post);
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                db.SaveChanges();
                TempData["Success"] = $"{addedCount} adet yeni SEO uyumlu blog yazısı eklendi.";
            }
            else
            {
                TempData["Info"] = "Tüm örnek içerikler zaten mevcut.";
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/Blog/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var blog = db.TblBlog.Find(id);
            if (blog != null)
            {
                string baslik = blog.Baslik;
                db.TblBlog.Remove(blog);
                db.SaveChanges();
                LogHelper.LogAudit("Silme", "TblBlog", id, $"Blog silindi: {baslik}");
                TempData["Success"] = "Blog yazısı silindi.";
            }
            return RedirectToAction("Index");
        }

        // Helper: Slug oluşturma
        private string CreateSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            
            text = text.ToLowerInvariant();
            text = text.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                       .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
            text = text.Substring(0, text.Length <= 75 ? text.Length : 75).Trim();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s", "-");
            
            return text;
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
