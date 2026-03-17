using System.Collections.Generic;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Models
{
    public class LayoutViewModel
    {
        public TblAyarlar Ayarlar { get; set; }
        public TblSeo Seo { get; set; }
        public List<TblSayfalar> Menu { get; set; }
        public List<TblHizmetler> Hizmetler { get; set; }
        public List<TblMarkalar> Markalar { get; set; }
        public List<TblIcerikler> Icerikler { get; set; }
        public List<TblBlog> Bloglar { get; set; }
        public List<TblGaleri> Galeri { get; set; }
        public List<TblBlogKategori> Kategoriler { get; set; }
        public TblBlog BlogDetay { get; set; }
        public List<TblSSS> SSS { get; set; }
        public List<TblMusteriYorumlari> Yorumlar { get; set; }
        public List<TblHizmetBolgeleri> HizmetBolgeleri { get; set; }
        public List<TblKuponlar> GecerliKuponlar { get; set; }
        public List<TblSosyalKanit> SosyalKanitlar { get; set; }
    }
}
