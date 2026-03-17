using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Models;

namespace AIROSWEB.Controllers
{
    public class BlogController : BaseController
    {
        // GET: Blog
        public ActionResult Index(int? kategoriId, string slug)
        {
            var model = GetBaseModel("Blog");
            
            var query = db.TblBlog.Where(x => x.AktifMi).OrderByDescending(x => x.YayinTarihi).AsQueryable();

            if (!string.IsNullOrEmpty(slug))
            {
                slug = slug.Trim().ToLower();
                var category = db.TblBlogKategori.ToList().FirstOrDefault(x => x.Slug.Trim().ToLower() == slug);
                if (category != null)
                {
                    kategoriId = category.Id;
                    ViewBag.ActiveCategory = category.KategoriAdi;
                    query = query.Where(x => x.KategoriId == category.Id);
                }
            }
            else if (kategoriId.HasValue)
            {
                query = query.Where(x => x.KategoriId == kategoriId.Value);
                var category = db.TblBlogKategori.Find(kategoriId);
                ViewBag.ActiveCategory = category?.KategoriAdi;
            }

            model.Bloglar = query.ToList();
            return View(model);
        }

        // GET: Blog/Detay/slug
        public ActionResult Detay(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            var model = GetBaseModel("Blog");
            var blog = db.TblBlog.ToList().FirstOrDefault(x => x.Slug.Trim().ToLower() == id.Trim().ToLower() && x.AktifMi);

            if (blog == null) return RedirectToAction("SayfaBulunamadi", "Home");

            // Görüntüleme sayısını artır
            blog.GoruntulemeSayisi += 1;
            db.SaveChanges();

            model.BlogDetay = blog;
            // Yorumları modele ekle (Sadece onaylılar)
            model.BlogDetay.Yorumlar = db.TblBlogYorum.Where(x => x.BlogId == blog.Id && x.OnayliMi).OrderByDescending(x => x.Tarih).ToList();

            model.Seo = new TblSeo 
            { 
                Title = blog.MetaTitle ?? blog.Baslik, 
                Description = blog.MetaDescription ?? blog.Ozet 
            };
            
            if (!string.IsNullOrEmpty(blog.ResimYolu)) { ViewBag.OgImage = blog.ResimYolu; }

            // Diğer ilgi çekici yazılar
            model.Bloglar = db.TblBlog.Where(x => x.AktifMi && x.Id != blog.Id)
                                     .OrderByDescending(x => x.YayinTarihi)
                                     .Take(3).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YorumYap(int BlogId, string AdSoyad, string Email, string Yorum)
        {
            if (string.IsNullOrEmpty(AdSoyad) || string.IsNullOrEmpty(Yorum))
            {
                TempData["Error"] = "Lütfen adınızı ve yorumunuzu yazınız.";
                return Redirect(Request.UrlReferrer.ToString());
            }

            var yeniYorum = new TblBlogYorum
            {
                BlogId = BlogId,
                AdSoyad = AdSoyad,
                Email = Email,
                Yorum = Yorum,
                Tarih = DateTime.Now,
                OnayliMi = false // Admin onayı gerekli
            };

            db.TblBlogYorum.Add(yeniYorum);
            db.SaveChanges();

            TempData["Success"] = "Yorumunuz başarıyla gönderildi. Yönetici onayından sonra yayınlanacaktır.";
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
