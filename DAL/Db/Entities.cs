using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace AIROSWEB.DAL.Db
{
    // Veritabanı ile eşleşen Entity Sınıfları

    
    public class TblAyarlar
    {
        [Key]
        public int Id { get; set; }
        public string SiteBaslik { get; set; }
        public string LogoYolu { get; set; }
        public string FooterLogoYolu { get; set; }
        public string FaviconYolu { get; set; }
        public string Telefon { get; set; }
        public string Telefon2 { get; set; }
        public string WhatsApp { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public string HaritaEmbed { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string YouTube { get; set; }
        public string FooterAciklama { get; set; }
        public string CalismaGunleri { get; set; }
        public string CalismaSaatleri { get; set; }
        public string GoogleAnalyticsId { get; set; }
        public string GoogleVerificationCode { get; set; }
        public string PwaThemeColor { get; set; }
        public string PwaAppShortName { get; set; }

        public bool? PopupAktif { get; set; }
        public string PopupBaslik { get; set; }
        public string PopupIcerik { get; set; }
        public int? PopupGecikme { get; set; }
        public int? PopupTekrarSuresi { get; set; }

        // Yeni Eklenen Gelişmiş Özellikler
        public bool? StickyBarAktif { get; set; }
        public string StickyBarMetin { get; set; }
        public string StickyBarBaslangic { get; set; } // Örn: 09:00
        public string StickyBarBitis { get; set; } // Örn: 20:00
        public string GoogleMapsYorumId { get; set; } // Google yorumları için ID veya Link
        
        public bool? ExitPopupAktif { get; set; }
        public int? ExitPopupGecikme { get; set; } // Saniye cinsinden
        public int? ExitPopupTekrarSuresi { get; set; } // Dakika cinsinden
        
        // Sosyal Kanıt Ayarları (Saniye Cinsinden)
        public int? SosyalKanitGecikme { get; set; } // İlk Çıkma Gecikmesi (Saniye)
        public int? SosyalKanitTekrar { get; set; } // Çıkış Sıklığı (Saniye)

        // --- YENİ EKLENEN ÖZELLİKLER ---
        public bool BakimModu { get; set; }
        public string BakimMesaji { get; set; }
        public string WhatsAppMesaj { get; set; }
        [AllowHtml]
        public string HeaderKod { get; set; } // <head> içine eklenecek kodlar
        [AllowHtml]
        public string FooterKod { get; set; } // </body> öncesine eklenecek kodlar
        public string InstagramToken { get; set; }
        public bool NewsletterAktif { get; set; }
    }

    // İşlem Günlüğü (Audit Logs)
    
    public class TblAuditLogs
    {
        [Key]
        public int Id { get; set; }
        public string Kullanici { get; set; }
        public string Islem { get; set; } // Ekleme, Güncelleme, Silme
        public string Tablo { get; set; }
        public int? KayitId { get; set; }
        public DateTime Tarih { get; set; }
        public string Detay { get; set; }
        public string Ip { get; set; }
    }

    // Güvenlik Günlüğü (Security Logs)
    
    public class TblSecurityLogs
    {
        [Key]
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string Islem { get; set; } // Giriş, Çıkış, Hatalı Giriş
        public DateTime Tarih { get; set; }
        public string Ip { get; set; }
        public string Durum { get; set; } // Başarılı, Başarısız
        public string Tarayici { get; set; }
    }

    // E-Bülten (Newsletter)
    
    public class TblNewsletter
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool AktifMi { get; set; }
        public bool Okundu { get; set; }
    }

    
    public class TblSayfalar
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Baslik { get; set; }
        public string Link { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
        public string Konum { get; set; } // Header, Footer, Both
        
        [System.Web.Mvc.AllowHtml]
        public string Icerik { get; set; } // İçerik metni (HTML)
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }

        public string ResimYolu { get; set; } // Banner / Kapak Resmi
        public int? UstMenuId { get; set; } // Dropdown için Alt Sayfa Mantığı
        public string Ozet { get; set; } // Kısa Açıklama Alanı
        public string Sablon { get; set; } // Tema / Şablon Seçimi
        public DateTime? YayinTarihi { get; set; } // İleri Tarihli Yayınlama
        
        //[System.ComponentModel.DataAnnotations.Schema.ForeignKey("UstMenuId")]
        public virtual TblSayfalar UstMenu { get; set; }
        
        public virtual ICollection<TblSayfalar> AltMenuler { get; set; }
    }

    
    public class TblIcerikler
    {
        [Key]
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string AltBaslik { get; set; }
        public string Aciklama { get; set; }
        public string ResimYolu { get; set; }
        public string BolumAdi { get; set; } 
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    }

    
    public class TblHizmetler
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public string Ikon { get; set; }
        public string Icerik { get; set; } 
        public string ResimYolu { get; set; } 
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public bool? AktifMi { get; set; }
    }

    
    public class TblMarkalar
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string MarkaAdi { get; set; }
        public string LogoYolu { get; set; }
        public bool? AktifMi { get; set; }
    }

    
    public class TblAdmin
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string KullaniciAdi { get; set; }
        [Required]
        public string Sifre { get; set; }
        public string Email { get; set; }
        public string SifreSifirlamaToken { get; set; }
        public DateTime? SifreSifirlamaTokenTarih { get; set; }
        public string GizliSoru { get; set; }
        public string GizliCevap { get; set; }
        public string Rol { get; set; }
        public bool? AktifMi { get; set; }
    }

    
    public class TblSeo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SayfaAdi { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
    }

    // İletişim Mesajları
    
    public class TblMesajlar
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Konu { get; set; }
        [Required]
        public string Mesaj { get; set; }
        public DateTime Tarih { get; set; }
        public bool Okundu { get; set; }
        public bool Yanitlandi { get; set; }
        public string YoneticiNotu { get; set; }
    }

    // Galeri / Projeler
    
    public class TblGaleri
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public string ResimYolu { get; set; }
        public string Kategori { get; set; } 
        public DateTime? Tarih { get; set; }
        public string Konum { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    }


    
    public class TblSSS
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Soru { get; set; }
        [Required]
        [System.Web.Mvc.AllowHtml]
        public string Cevap { get; set; }
        public string Kategori { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    }


    
    public class TblBlogKategori
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string KategoriAdi { get; set; }
        public string Slug { get; set; }
        public bool AktifMi { get; set; }
    }

  
    
    public class TblBlog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Baslik { get; set; }
        public string Slug { get; set; }
        public string Ozet { get; set; }
        [AllowHtml]
        public string Icerik { get; set; }
        public string ResimYolu { get; set; }
        public int? KategoriId { get; set; }
        public string Yazar { get; set; }
        public DateTime YayinTarihi { get; set; }
        public int GoruntulemeSayisi { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public bool AktifMi { get; set; }
        
        public virtual ICollection<TblBlogYorum> Yorumlar { get; set; }
    }


    
    public class TblRandevular
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string AdSoyad { get; set; }
        [Required]
        public string Telefon { get; set; }
        public string Email { get; set; }
        public int? HizmetId { get; set; }
        public string HizmetAdi { get; set; }
        public DateTime? RandevuTarihi { get; set; }
        public string RandevuSaati { get; set; } // Örn: 09:00 - 12:00
        public string Adres { get; set; }
        public string Mesaj { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public string Durum { get; set; } // Beklemede, Onaylandı, Tamamlandı, İptal
        public string YoneticiNotu { get; set; }
        public string KuponKodu { get; set; }
        public int? TeknisyenId { get; set; }
        public string AlinanUcret { get; set; }
    }


    public class TblKuponlar
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Kod { get; set; }
        public string Aciklama { get; set; }
        public int IndirimOrani { get; set; } // Yüzde veya miktar olarak
        public int Miktar { get; set; } // Sabit indirim
        public DateTime? SonKullanmaTarihi { get; set; }
        public int KullanimSayisi { get; set; }
        public bool AktifMi { get; set; }
    }


    public class TblMusteriYorumlari
    {
        [Key]
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string Meslek { get; set; }
        public string Yorum { get; set; }
        public int Puan { get; set; }
        public string ResimYolu { get; set; }
        public bool AktifMi { get; set; }
        public DateTime Tarih { get; set; }
    }


    
    public class TblBlogYorum
    {
        [Key]
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public string Yorum { get; set; }
        public DateTime Tarih { get; set; }
        public bool OnayliMi { get; set; }
        public string YoneticiCevabi { get; set; }
        public DateTime? CevapTarihi { get; set; }
        
        
        public virtual TblBlog Blog { get; set; }
    }

    
    public class TblHizmetBolgeleri
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BolgeAdi { get; set; }
        public string Slug { get; set; }
        public string Aciklama { get; set; } // O bölgeye özel kısa metni
        public string Icerik { get; set; } // O bölgeye özel detaylı SEO metni
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public bool AktifMi { get; set; }
        public int Sira { get; set; }
    }

    public class TblArizaFiyatlari
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CihazTuru { get; set; }
        public string Marka { get; set; }
        [Required]
        public string ArizaAdi { get; set; }
        [Required]
        public string FiyatAraligi { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    }

    public class TblSosyalKanit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string MusteriAdi { get; set; } // Örn: "Mustafa C."
        public string Lokasyon { get; set; } // Örn: "Mudanya"
        [Required]
        public string Islem { get; set; } // Örn: "WhatsApp üzerinden teklif aldı."
        public string Zaman { get; set; } // Örn: "47 dk önce"
        public string Tip { get; set; } // Örn: "whatsapp" veya "star"
        public bool AktifMi { get; set; }
        public int Sira { get; set; }
        public DateTime? OlusturulmaTarihi { get; set; }
    }
}
