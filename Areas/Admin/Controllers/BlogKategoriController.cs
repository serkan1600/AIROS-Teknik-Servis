using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class BlogKategoriController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/BlogKategori
        public ActionResult Index()
        {
            var kategoriler = db.TblBlogKategori.ToList();
            return View(kategoriler);
        }

        // GET: Admin/BlogKategori/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/BlogKategori/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblBlogKategori model)
        {
            if (ModelState.IsValid)
            {
                if (db.TblBlogKategori.Any(x => x.KategoriAdi == model.KategoriAdi))
                {
                    TempData["Error"] = "Bu isimde bir kategori zaten mevcut.";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(model.Slug))
                {
                    model.Slug = CreateSlug(model.KategoriAdi);
                }
                
                db.TblBlogKategori.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Kategori başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Lütfen tüm alanları doğru doldurduğunuzdan emin olun.";
            return RedirectToAction("Index");
        }

        // GET: Admin/BlogKategori/Edit/5
        public ActionResult Edit(int id)
        {
            var kategori = db.TblBlogKategori.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }

        // POST: Admin/BlogKategori/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblBlogKategori model)
        {
            if (ModelState.IsValid)
            {
                var kategori = db.TblBlogKategori.Find(model.Id);
                if (kategori != null)
                {
                    kategori.KategoriAdi = model.KategoriAdi;
                    kategori.Slug = string.IsNullOrEmpty(model.Slug) ? CreateSlug(model.KategoriAdi) : model.Slug;
                    kategori.AktifMi = model.AktifMi;
                    
                    db.SaveChanges();
                    TempData["Success"] = "Kategori güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // POST: Admin/BlogKategori/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var kategori = db.TblBlogKategori.Find(id);
            if (kategori != null)
            {
                // Kategoriye ait blog yazısı var mı kontrol et
                var blogSayisi = db.TblBlog.Count(x => x.KategoriId == id);
                if (blogSayisi > 0)
                {
                    TempData["Error"] = $"Bu kategoriye ait {blogSayisi} blog yazısı var. Önce blog yazılarını silin veya başka kategoriye taşıyın.";
                }
                else
                {
                    db.TblBlogKategori.Remove(kategori);
                    db.SaveChanges();
                    TempData["Success"] = "Kategori silindi.";
                }
            }
            return RedirectToAction("Index");
        }

        private string CreateSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            
            text = text.ToLowerInvariant();
            text = text.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                       .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
            text = text.Substring(0, text.Length <= 50 ? text.Length : 50).Trim();
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
