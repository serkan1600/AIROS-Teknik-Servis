using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace AIROSWEB.DAL.Db
{
    public class AirosTeknikServisEntities : DbContext
    {
        public AirosTeknikServisEntities()
            : base("name=AirosTeknikServisEntities")
        {
            Database.SetInitializer<AirosTeknikServisEntities>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }

        public virtual DbSet<TblAdmin> TblAdmin { get; set; }
        public virtual DbSet<TblHizmetler> TblHizmetler { get; set; }
        public virtual DbSet<TblMarkalar> TblMarkalar { get; set; }
        public virtual DbSet<TblSeo> TblSeo { get; set; }
        public virtual DbSet<TblAyarlar> TblAyarlar { get; set; }
        public virtual DbSet<TblSayfalar> TblSayfalar { get; set; }
        public virtual DbSet<TblIcerikler> TblIcerikler { get; set; }
        public virtual DbSet<TblMesajlar> TblMesajlar { get; set; }
        public virtual DbSet<TblGaleri> TblGaleri { get; set; }
        public virtual DbSet<TblSSS> TblSSS { get; set; }
        public virtual DbSet<TblBlogKategori> TblBlogKategori { get; set; }
        public virtual DbSet<TblBlog> TblBlog { get; set; }
        public virtual DbSet<TblRandevular> TblRandevular { get; set; }
        public virtual DbSet<TblMusteriYorumlari> TblMusteriYorumlari { get; set; }
        public virtual DbSet<TblBlogYorum> TblBlogYorum { get; set; }
        public virtual DbSet<TblHizmetBolgeleri> TblHizmetBolgeleri { get; set; }
        public virtual DbSet<TblAuditLogs> TblAuditLogs { get; set; }
        public virtual DbSet<TblSecurityLogs> TblSecurityLogs { get; set; }
        public virtual DbSet<TblNewsletter> TblNewsletter { get; set; }
        public virtual DbSet<TblKuponlar> TblKuponlar { get; set; }
        public virtual DbSet<TblArizaFiyatlari> TblArizaFiyatlari { get; set; }
        public virtual DbSet<TblSosyalKanit> TblSosyalKanit { get; set; }
    }
}
