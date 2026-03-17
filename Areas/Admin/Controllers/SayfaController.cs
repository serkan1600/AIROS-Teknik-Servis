using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class SayfaController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GM: Admin/Sayfa
        public ActionResult Index(bool reseed = false)
        {
            if (reseed || !db.TblSayfalar.Any())
            {
                CheckAndSeedPages();
            }

            var values = db.TblSayfalar.AsNoTracking().OrderBy(x => x.Sira).ToList();
            return View(values);
        }

        private void CheckAndSeedPages()
        {
            try 
            {
                // 1. Mükerrerleri Temizle (Hem Başlık Hem Link Bazlı)
                var allPages = db.TblSayfalar.ToList();
                
                // Başlık bazlı temizlik
                var titleGroups = allPages.GroupBy(x => x.Baslik?.Trim().ToLower()).Where(g => g.Count() > 1);
                foreach (var group in titleGroups)
                {
                    var keep = group.OrderBy(x => x.Id).First();
                    db.TblSayfalar.RemoveRange(group.Where(x => x.Id != keep.Id));
                }
                db.SaveChanges();

                // Link bazlı temizlik (Örn: /Blog ve /Blog/Index çakışması)
                allPages = db.TblSayfalar.ToList(); // Listeyi güncelle
                var linkGroups = allPages.GroupBy(x => x.Link?.Trim().ToLower().Replace("/index", "")).Where(g => g.Count() > 1);
                foreach (var group in linkGroups)
                {
                    var keep = group.OrderBy(x => x.Id).First();
                    db.TblSayfalar.RemoveRange(group.Where(x => x.Id != keep.Id));
                }
                db.SaveChanges();

                // 2. Eksik Menü Öğelerini Tanımla
                var coreMenus = new List<TblSayfalar>
                {
                    new TblSayfalar { Baslik = "Anasayfa", Link = "/", Sira = 1, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "Hizmetler", Link = "/Home/Hizmetler", Sira = 2, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "Hakkımızda", Link = "/Home/Hakkimizda", Sira = 3, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "Galeri", Link = "/Home/Galeri", Sira = 4, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "Blog", Link = "/Blog", Sira = 5, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "S.S.S", Link = "/Home/SSS", Sira = 6, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "İletişim", Link = "/Home/Iletisim", Sira = 7, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "Servis Çağır", Link = "/Home/Randevu", Sira = 8, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "Sosyal Medya", Link = "#social", Sira = 9, Konum = "Header", AktifMi = true },
                    new TblSayfalar { Baslik = "Telefon Hattı", Link = "tel:05010022516", Sira = 10, Konum = "Header", AktifMi = true }
                };

                foreach (var m in coreMenus)
                {
                    var normalizedLink = m.Link.ToLower().Trim().Replace("/index", "");
                    if (!db.TblSayfalar.ToList().Any(x => 
                        x.Baslik.ToLower().Trim() == m.Baslik.ToLower().Trim() || 
                        x.Link.ToLower().Trim().Replace("/index", "") == normalizedLink))
                    {
                        db.TblSayfalar.Add(m);
                    }
                }
                db.SaveChanges();
            }
            catch { }
        }

        private void PrepareViewBags(int? seciliUstMenuId = null, string sablon = null)
        {
            var q = db.TblSayfalar.Where(x => x.UstMenuId == null).OrderBy(x => x.Sira).ToList();
            ViewBag.UstMenuler = new SelectList(q, "Id", "Baslik", seciliUstMenuId);
            ViewBag.Sablonlar = new SelectList(new[] { "Standart", "SagsTarafResimli", "TamSayfa", "IletisimFormlu" }, sablon);
        }

        [HttpGet]
        public ActionResult Create()
        {
            PrepareViewBags();
            return View(new TblSayfalar { Sablon = "Standart", Sira = 0, Konum = "Header" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblSayfalar p, System.Web.HttpPostedFileBase kapakResmi)
        {
            if (ModelState.IsValid)
            {
                if (kapakResmi != null && kapakResmi.ContentLength > 0)
                {
                    string fileName = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(kapakResmi.FileName);
                    string dirPath = Server.MapPath("~/Content/Uploads/Images/");
                    if (!System.IO.Directory.Exists(dirPath))
                    {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }
                    string path = System.IO.Path.Combine(dirPath, fileName);
                    kapakResmi.SaveAs(path);
                    p.ResimYolu = "/Content/Uploads/Images/" + fileName;
                }
                
                p.AktifMi = true;
                db.TblSayfalar.Add(p);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
                return RedirectToAction("Index");
            }
            PrepareViewBags(p.UstMenuId, p.Sablon);
            return View(p);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var value = db.TblSayfalar.Find(id);
            if (value == null) return HttpNotFound();
            PrepareViewBags(value.UstMenuId, value.Sablon);
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblSayfalar p, System.Web.HttpPostedFileBase kapakResmi, bool resmiSil = false)
        {
            var value = db.TblSayfalar.Find(p.Id);
            if (value == null) return HttpNotFound();

            if (resmiSil)
            {
                if (!string.IsNullOrEmpty(value.ResimYolu))
                {
                    string oldPath = Server.MapPath("~" + value.ResimYolu);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                value.ResimYolu = null;
            }
            else if (kapakResmi != null && kapakResmi.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(value.ResimYolu))
                {
                    string oldPath = Server.MapPath("~" + value.ResimYolu);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                string fileName = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(kapakResmi.FileName);
                string dirPath = Server.MapPath("~/Content/Uploads/Images/");
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                string path = System.IO.Path.Combine(dirPath, fileName);
                kapakResmi.SaveAs(path);
                value.ResimYolu = "/Content/Uploads/Images/" + fileName;
            }

            value.Baslik = p.Baslik;
            value.Link = p.Link;
            value.Sira = p.Sira;
            value.Konum = p.Konum;
            value.AktifMi = p.AktifMi;
            value.Icerik = p.Icerik;
            value.MetaTitle = p.MetaTitle;
            value.MetaDescription = p.MetaDescription;
            value.UstMenuId = p.UstMenuId;
            value.Ozet = p.Ozet;
            value.Sablon = p.Sablon;
            value.YayinTarihi = p.YayinTarihi;
            
            db.SaveChanges();
            CacheHelper.ClearLayoutCache();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var value = db.TblSayfalar.Find(id);
            if (value != null)
            {
                // Silme oncesi alt menuleri guncelle
                var altMenuler = db.TblSayfalar.Where(x => x.UstMenuId == value.Id).ToList();
                foreach (var alt in altMenuler) alt.UstMenuId = null;
                
                db.TblSayfalar.Remove(value);
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DurumDegistir(int id)
        {
            var value = db.TblSayfalar.Find(id);
            if (value != null)
            {
                value.AktifMi = !value.AktifMi;
                db.SaveChanges();
                CacheHelper.ClearLayoutCache();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateOrder(List<int> itemIds)
        {
            try
            {
                if (itemIds != null && itemIds.Count > 0)
                {
                    int index = 1;
                    foreach (var id in itemIds)
                    {
                        var item = db.TblSayfalar.Find(id);
                        if (item != null)
                        {
                            item.Sira = index++;
                        }
                    }
                    db.SaveChanges();
                    CacheHelper.ClearLayoutCache();
                    return Json(new { success = true });
                }
            }
            catch { }
            return Json(new { success = false });
        }
    }
}

