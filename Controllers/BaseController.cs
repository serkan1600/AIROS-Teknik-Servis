using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using AIROSWEB.Models;

namespace AIROSWEB.Controllers
{
    public class BaseController : Controller
    {
        protected AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        protected LayoutViewModel GetBaseModel(string sayfaAdi = "Genel")
        {
            // Önbellek anahtarı
            string cacheKey = "LayoutModel_" + (sayfaAdi ?? "Genel");
            
            // Önbellekte var mı kontrol et
            if (HttpRuntime.Cache[cacheKey] is LayoutViewModel cachedModel)
            {
                return cachedModel;
            }

            var model = new LayoutViewModel();
            try { model.Ayarlar = db.TblAyarlar.AsNoTracking().FirstOrDefault() ?? new TblAyarlar(); } catch { model.Ayarlar = new TblAyarlar(); }
            try { model.Seo = db.TblSeo.AsNoTracking().FirstOrDefault(x => x.SayfaAdi == sayfaAdi); } catch { }
            try { model.Menu = db.TblSayfalar.AsNoTracking().Where(x => x.AktifMi).OrderBy(x => x.Sira).ToList(); } catch { model.Menu = new List<TblSayfalar>(); }
            try { model.Hizmetler = db.TblHizmetler.AsNoTracking().Where(x => x.AktifMi == true).ToList(); } catch { model.Hizmetler = new List<TblHizmetler>(); }
            try { model.Icerikler = db.TblIcerikler.AsNoTracking().Where(x => x.AktifMi).OrderBy(x => x.Sira).ThenByDescending(x => x.Id).ToList(); } catch { model.Icerikler = new List<TblIcerikler>(); }
            try { model.Kategoriler = db.TblBlogKategori.AsNoTracking().Where(x => x.AktifMi).ToList(); } catch { model.Kategoriler = new List<TblBlogKategori>(); }
            try
            {
                model.HizmetBolgeleri = db.TblHizmetBolgeleri.AsNoTracking().Where(x => x.AktifMi).OrderBy(x => x.Sira).ToList();
            }
            catch 
            {
                model.HizmetBolgeleri = new List<TblHizmetBolgeleri>();
            }

            try 
            {
                var suan = DateTime.Now;
                model.GecerliKuponlar = db.TblKuponlar.AsNoTracking().Where(x => x.AktifMi == true && (!x.SonKullanmaTarihi.HasValue || x.SonKullanmaTarihi > suan)).ToList();
            } 
            catch 
            {
                model.GecerliKuponlar = new List<TblKuponlar>();
            }
            
            try 
            {
                model.SosyalKanitlar = db.TblSosyalKanit.AsNoTracking().Where(x => x.AktifMi == true).OrderBy(x => x.Sira).ToList();
            }
            catch 
            {
                model.SosyalKanitlar = new List<TblSosyalKanit>();
            }

            // 15 dakika boyunca önbelleğe al
            HttpRuntime.Cache.Insert(cacheKey, model, null, DateTime.Now.AddMinutes(15), System.Web.Caching.Cache.NoSlidingExpiration);

            return model;
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
