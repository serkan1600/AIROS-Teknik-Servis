using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class SetupController : Controller
    {
        private AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RunSetup()
        {
            int changes = 0;
            var tableCreationScripts = new Dictionary<string, string>
            {
                { "TblAyarlar", @"
                    CREATE TABLE [dbo].[TblAyarlar](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [SiteBaslik] [nvarchar](max) NULL,
                        [LogoYolu] [nvarchar](max) NULL,
                        [FooterLogoYolu] [nvarchar](max) NULL,
                        [FaviconYolu] [nvarchar](max) NULL,
                        [Telefon] [nvarchar](max) NULL,
                        [Telefon2] [nvarchar](max) NULL,
                        [WhatsApp] [nvarchar](max) NULL,
                        [Email] [nvarchar](max) NULL,
                        [Adres] [nvarchar](max) NULL,
                        [HaritaEmbed] [nvarchar](max) NULL,
                        [Facebook] [nvarchar](max) NULL,
                        [Instagram] [nvarchar](max) NULL,
                        [Twitter] [nvarchar](max) NULL,
                        [LinkedIn] [nvarchar](max) NULL,
                        [YouTube] [nvarchar](max) NULL,
                        [FooterAciklama] [nvarchar](max) NULL,
                        [CalismaGunleri] [nvarchar](max) NULL,
                        [CalismaSaatleri] [nvarchar](max) NULL,
                        [GoogleAnalyticsId] [nvarchar](max) NULL,
                        [GoogleVerificationCode] [nvarchar](max) NULL,
                        [PwaThemeColor] [nvarchar](max) NULL,
                        [PwaAppShortName] [nvarchar](max) NULL,
                        [PopupAktif] [bit] NULL,
                        [PopupBaslik] [nvarchar](max) NULL,
                        [PopupIcerik] [nvarchar](max) NULL,
                        [StickyBarAktif] [bit] NULL,
                        [StickyBarMetin] [nvarchar](max) NULL,
                        [GoogleMapsYorumId] [nvarchar](max) NULL,
                        [ExitPopupAktif] [bit] NULL,
                        [ExitPopupTekrarSuresi] [int] NULL,
                        CONSTRAINT [PK_dbo.TblAyarlar] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )" 
                },
                { "TblHizmetBolgeleri", @"
                    CREATE TABLE [dbo].[TblHizmetBolgeleri](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [BolgeAdi] [nvarchar](max) NOT NULL,
                        [Slug] [nvarchar](max) NULL,
                        [Aciklama] [nvarchar](max) NULL,
                        [Icerik] [nvarchar](max) NULL,
                        [MetaTitle] [nvarchar](max) NULL,
                        [MetaDescription] [nvarchar](max) NULL,
                        [Keywords] [nvarchar](max) NULL,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        [Sira] [int] NOT NULL DEFAULT 0,
                        CONSTRAINT [PK_dbo.TblHizmetBolgeleri] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )" 
                },
                { "TblAdmin", @"
                    CREATE TABLE [dbo].[TblAdmin](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [KullaniciAdi] [nvarchar](max) NOT NULL,
                        [Sifre] [nvarchar](max) NOT NULL,
                        [Email] [nvarchar](max) NULL,
                        [SifreSifirlamaToken] [nvarchar](max) NULL,
                        [SifreSifirlamaTokenTarih] [datetime] NULL,
                        [GizliSoru] [nvarchar](max) NULL,
                        [GizliCevap] [nvarchar](max) NULL,
                        [Rol] [nvarchar](max) NULL,
                        [AktifMi] [bit] NULL,
                        CONSTRAINT [PK_dbo.TblAdmin] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )" 
                },
                { "TblHizmetler", @"
                    CREATE TABLE [dbo].[TblHizmetler](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Baslik] [nvarchar](max) NOT NULL,
                        [Aciklama] [nvarchar](max) NULL,
                        [Ikon] [nvarchar](max) NULL,
                        [Icerik] [nvarchar](max) NULL,
                        [ResimYolu] [nvarchar](max) NULL,
                        [MetaTitle] [nvarchar](max) NULL,
                        [MetaDescription] [nvarchar](max) NULL,
                        [Keywords] [nvarchar](max) NULL,
                        [AktifMi] [bit] NULL,
                        CONSTRAINT [PK_dbo.TblHizmetler] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblIcerikler", @"
                    CREATE TABLE [dbo].[TblIcerikler](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Baslik] [nvarchar](max) NULL,
                        [AltBaslik] [nvarchar](max) NULL,
                        [Aciklama] [nvarchar](max) NULL,
                        [ResimYolu] [nvarchar](max) NULL,
                        [BolumAdi] [nvarchar](max) NULL,
                        [Sira] [int] NOT NULL DEFAULT 0,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_dbo.TblIcerikler] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblMarkalar", @"
                    CREATE TABLE [dbo].[TblMarkalar](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [MarkaAdi] [nvarchar](max) NOT NULL,
                        [LogoYolu] [nvarchar](max) NULL,
                        [AktifMi] [bit] NULL,
                        CONSTRAINT [PK_dbo.TblMarkalar] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblGaleri", @"
                    CREATE TABLE [dbo].[TblGaleri](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Baslik] [nvarchar](max) NOT NULL,
                        [Aciklama] [nvarchar](max) NULL,
                        [ResimYolu] [nvarchar](max) NULL,
                        [Kategori] [nvarchar](max) NULL,
                        [Tarih] [datetime] NULL,
                        [Konum] [nvarchar](max) NULL,
                        [Sira] [int] NOT NULL DEFAULT 0,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_dbo.TblGaleri] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblBlogKategori", @"
                    CREATE TABLE [dbo].[TblBlogKategori](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [KategoriAdi] [nvarchar](max) NOT NULL,
                        [Slug] [nvarchar](max) NULL,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_dbo.TblBlogKategori] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblBlog", @"
                    CREATE TABLE [dbo].[TblBlog](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Baslik] [nvarchar](max) NOT NULL,
                        [Slug] [nvarchar](max) NULL,
                        [Ozet] [nvarchar](max) NULL,
                        [Icerik] [nvarchar](max) NULL,
                        [ResimYolu] [nvarchar](max) NULL,
                        [KategoriId] [int] NULL,
                        [Yazar] [nvarchar](max) NULL,
                        [YayinTarihi] [datetime] NOT NULL,
                        [GoruntulemeSayisi] [int] NOT NULL DEFAULT 0,
                        [MetaTitle] [nvarchar](max) NULL,
                        [MetaDescription] [nvarchar](max) NULL,
                        [Keywords] [nvarchar](max) NULL,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_dbo.TblBlog] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblMesajlar", @"
                    CREATE TABLE [dbo].[TblMesajlar](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [AdSoyad] [nvarchar](max) NOT NULL,
                        [Email] [nvarchar](max) NULL,
                        [Telefon] [nvarchar](max) NULL,
                        [Konu] [nvarchar](max) NULL,
                        [Mesaj] [nvarchar](max) NOT NULL,
                        [Tarih] [datetime] NOT NULL,
                        [Okundu] [bit] NOT NULL DEFAULT 0,
                        [Yanitlandi] [bit] NOT NULL DEFAULT 0,
                        [YoneticiNotu] [nvarchar](max) NULL,
                        CONSTRAINT [PK_dbo.TblMesajlar] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblSSS", @"
                    CREATE TABLE [dbo].[TblSSS](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Soru] [nvarchar](max) NOT NULL,
                        [Cevap] [nvarchar](max) NOT NULL,
                        [Sira] [int] NOT NULL DEFAULT 0,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_dbo.TblSSS] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblMusteriYorumlari", @"
                    CREATE TABLE [dbo].[TblMusteriYorumlari](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [AdSoyad] [nvarchar](max) NOT NULL,
                        [Yorum] [nvarchar](max) NOT NULL,
                        [Puan] [int] NOT NULL DEFAULT 5,
                        [Tarih] [datetime] NOT NULL,
                        [ResimYolu] [nvarchar](max) NULL,
                        [Meslek] [nvarchar](max) NULL,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_dbo.TblMusteriYorumlari] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblSeo", @"
                    CREATE TABLE [dbo].[TblSeo](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [SayfaAdi] [nvarchar](max) NOT NULL,
                        [Title] [nvarchar](max) NULL,
                        [Description] [nvarchar](max) NULL,
                        [Keywords] [nvarchar](max) NULL,
                        CONSTRAINT [PK_dbo.TblSeo] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblRandevular", @"
                    CREATE TABLE [dbo].[TblRandevular](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [AdSoyad] [nvarchar](max) NOT NULL,
                        [Telefon] [nvarchar](max) NOT NULL,
                        [Email] [nvarchar](max) NULL,
                        [HizmetId] [int] NULL,
                        [HizmetAdi] [nvarchar](max) NULL,
                        [RandevuTarihi] [datetime] NOT NULL,
                        [Adres] [nvarchar](max) NULL,
                        [Mesaj] [nvarchar](max) NULL,
                        [Durum] [nvarchar](max) NOT NULL DEFAULT 'Beklemede',
                        [OlusturmaTarihi] [datetime] NOT NULL,
                        CONSTRAINT [PK_dbo.TblRandevular] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblBlogYorum", @"
                    CREATE TABLE [dbo].[TblBlogYorum](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [BlogId] [int] NOT NULL,
                        [AdSoyad] [nvarchar](max) NOT NULL,
                        [Email] [nvarchar](max) NOT NULL,
                        [Yorum] [nvarchar](max) NOT NULL,
                        [Tarih] [datetime] NOT NULL,
                        [OnayliMi] [bit] NOT NULL DEFAULT 0,
                        CONSTRAINT [PK_dbo.TblBlogYorum] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblSayfalar", @"
                    CREATE TABLE [dbo].[TblSayfalar](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Baslik] [nvarchar](max) NOT NULL,
                        [Link] [nvarchar](max) NOT NULL,
                        [Sira] [int] NOT NULL DEFAULT 0,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        [Konum] [nvarchar](max) NULL,
                        CONSTRAINT [PK_dbo.TblSayfalar] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblAuditLogs", @"
                    CREATE TABLE [dbo].[TblAuditLogs](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Kullanici] [nvarchar](max) NULL,
                        [Islem] [nvarchar](max) NULL,
                        [Tablo] [nvarchar](max) NULL,
                        [KayitId] [int] NULL,
                        [Tarih] [datetime] NOT NULL,
                        [Detay] [nvarchar](max) NULL,
                        [Ip] [nvarchar](max) NULL,
                        CONSTRAINT [PK_dbo.TblAuditLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblSecurityLogs", @"
                    CREATE TABLE [dbo].[TblSecurityLogs](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [KullaniciAdi] [nvarchar](max) NULL,
                        [Islem] [nvarchar](max) NULL,
                        [Tarih] [datetime] NOT NULL,
                        [Ip] [nvarchar](max) NULL,
                        [Durum] [nvarchar](max) NULL,
                        [Tarayici] [nvarchar](max) NULL,
                        CONSTRAINT [PK_dbo.TblSecurityLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblNewsletter", @"
                    CREATE TABLE [dbo].[TblNewsletter](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Email] [nvarchar](max) NOT NULL,
                        [KayitTarihi] [datetime] NOT NULL,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        [Okundu] [bit] NOT NULL DEFAULT 0,
                        CONSTRAINT [PK_dbo.TblNewsletter] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                },
                { "TblKuponlar", @"
                    CREATE TABLE [dbo].[TblKuponlar](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Kod] [nvarchar](max) NOT NULL,
                        [Aciklama] [nvarchar](max) NULL,
                        [IndirimOrani] [int] NOT NULL DEFAULT 0,
                        [Miktar] [int] NOT NULL DEFAULT 0,
                        [SonKullanmaTarihi] [datetime] NULL,
                        [KullanimSayisi] [int] NOT NULL DEFAULT 0,
                        [AktifMi] [bit] NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_dbo.TblKuponlar] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )"
                }
            };
            
            // Eksik sütunları kontrol et ve ekle
            var columnChecks = new List<(string Table, string Column, string Type)>
            {
                ("TblAyarlar", "BakimModu", "bit NOT NULL DEFAULT 0"),
                ("TblAyarlar", "BakimMesaji", "nvarchar(max) NULL"),
                ("TblAyarlar", "WhatsAppMesaj", "nvarchar(max) NULL"),
                ("TblAyarlar", "HeaderKod", "nvarchar(max) NULL"),
                ("TblAyarlar", "FooterKod", "nvarchar(max) NULL"),
                ("TblAyarlar", "InstagramToken", "nvarchar(max) NULL"),
                ("TblAyarlar", "NewsletterAktif", "bit NOT NULL DEFAULT 1"),
                ("TblNewsletter", "Okundu", "bit NOT NULL DEFAULT 0"),
                ("TblRandevular", "KuponKodu", "nvarchar(max) NULL"),
                ("TblHizmetBolgeleri", "Icerik", "nvarchar(max) NULL"),
                ("TblHizmetBolgeleri", "MetaTitle", "nvarchar(max) NULL"),
                ("TblHizmetBolgeleri", "MetaDescription", "nvarchar(max) NULL"),
                ("TblHizmetBolgeleri", "Keywords", "nvarchar(max) NULL"),
                ("TblHizmetler", "MetaTitle", "nvarchar(max) NULL"),
                ("TblHizmetler", "MetaDescription", "nvarchar(max) NULL"),
                ("TblHizmetler", "Keywords", "nvarchar(max) NULL"),
                ("TblHizmetler", "AktifMi", "bit NOT NULL DEFAULT 1"),
                ("TblBlog", "Keywords", "nvarchar(max) NULL"),
                ("TblSayfalar", "AktifMi", "bit NOT NULL DEFAULT 1"),
                ("TblSayfalar", "Konum", "nvarchar(max) NULL"),
                ("TblIcerikler", "AktifMi", "bit NOT NULL DEFAULT 1"),
                ("TblSSS", "AktifMi", "bit NOT NULL DEFAULT 1")
            };

            foreach (var table in tableCreationScripts)
            {
                try
                {
                    db.Database.ExecuteSqlCommand($@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{table.Key}')
                        BEGIN
                            {table.Value}
                        END
                    ");
                }
                catch { }
            }

            foreach (var col in columnChecks)
            {
                try
                {
                    db.Database.ExecuteSqlCommand($@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{col.Table}' AND COLUMN_NAME = '{col.Column}')
                        BEGIN
                            ALTER TABLE [dbo].[{col.Table}] ADD [{col.Column}] {col.Type}
                        END
                    ");
                }
                catch { }
            }

            try
            {
                if (!db.TblAdmin.Any())
                {
                    db.TblAdmin.Add(new TblAdmin
                    {
                        KullaniciAdi = "admin",
                        Sifre = AIROSWEB.Helpers.HashHelper.ComputeSha256Hash("12345"), 
                        Email = "admin@airosteknik.com",
                        Rol = "Admin",
                        AktifMi = true,
                        GizliSoru = "İlk evcil hayvanınızın adı?",
                        GizliCevap = "Boncuk"
                    });
                    changes++;
                }
            } catch { }

            try
            {
                if (!db.TblAyarlar.Any())
                {
                    db.TblAyarlar.Add(new TblAyarlar
                    {
                        SiteBaslik = "AIROS Teknik Servis – Bursa Kombi ve Klima Servisi",
                        Telefon = "0501 002 25 16",
                        Email = "info@airosteknik.com",
                        Adres = "Emek, Yunus Emre Cd. No: 6, Osmangazi/Bursa",
                        CalismaGunleri = "Pazartesi - Cumartesi",
                        CalismaSaatleri = "08:00 - 20:00",
                        FooterAciklama = "Bursa kombi servisi, klima bakım, doğalgaz tesisatı ve yerden ısıtma sistemlerinde 7/24 profesyonel teknik servis hizmeti.",
                        NewsletterAktif = true
                    });
                    changes++;
                }
            } catch { }

            // 📍 SEED: HİZMET BÖLGELERİ (SEO İÇİN)
            try
            {
                if (!db.TblHizmetBolgeleri.Any())
                {
                    var bolgeler = new List<TblHizmetBolgeleri>
                    {
                        new TblHizmetBolgeleri { BolgeAdi = "Osmangazi", Slug = "osmangazi-kombi-servisi", Sira = 1, AktifMi = true, Aciklama = "Osmangazi bölgesi 7/24 kombi ve klima servisi." },
                        new TblHizmetBolgeleri { BolgeAdi = "Nilüfer", Slug = "nilufer-kombi-servisi", Sira = 2, AktifMi = true, Aciklama = "Nilüfer bölgesi profesyonel teknik destek." },
                        new TblHizmetBolgeleri { BolgeAdi = "Yıldırım", Slug = "yildirim-kombi-servisi", Sira = 3, AktifMi = true, Aciklama = "Yıldırım bölgesi hızlı servis kaydı." },
                        new TblHizmetBolgeleri { BolgeAdi = "Mudanya", Slug = "mudanya-kombi-servisi", Sira = 4, AktifMi = true, Aciklama = "Mudanya bölgesi klima bakım ve onarım." },
                        new TblHizmetBolgeleri { BolgeAdi = "Gürsu", Slug = "gursu-kombi-servisi", Sira = 5, AktifMi = true, Aciklama = "Gürsu bölgesi tesisat ve petek temizliği." }
                    };
                    db.TblHizmetBolgeleri.AddRange(bolgeler);
                    changes++;
                }
            } catch { }

            if (changes > 0) db.SaveChanges();
            
            TempData["Success"] = "Sistem kurulumu, veritabanı onarımı ve SEO alt yapısı başarıyla güncellendi.";
            return RedirectToAction("Index");
        }
    }
}
