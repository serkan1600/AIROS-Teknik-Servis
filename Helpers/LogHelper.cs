using System;
using System.Web;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Helpers
{
    public static class LogHelper
    {
        public static void LogAudit(string islem, string tablo, int? kayitId = null, string detay = null)
        {
            try
            {
                using (var db = new AirosTeknikServisEntities())
                {
                    var log = new TblAuditLogs
                    {
                        Kullanici = HttpContext.Current?.User?.Identity?.Name ?? "Sistem",
                        Islem = islem,
                        Tablo = tablo,
                        KayitId = kayitId,
                        Tarih = DateTime.Now,
                        Detay = detay,
                        Ip = HttpContext.Current?.Request?.UserHostAddress
                    };
                    db.TblAuditLogs.Add(log);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        public static void LogSecurity(string kullaniciAdi, string islem, string durum, string detay = null)
        {
            try
            {
                using (var db = new AirosTeknikServisEntities())
                {
                    var log = new TblSecurityLogs
                    {
                        KullaniciAdi = kullaniciAdi,
                        Islem = islem,
                        Durum = durum,
                        Tarih = DateTime.Now,
                        Ip = HttpContext.Current?.Request?.UserHostAddress,
                        Tarayici = HttpContext.Current?.Request?.UserAgent
                    };
                    db.TblSecurityLogs.Add(log);
                    db.SaveChanges();
                }
            }
            catch { }
        }
    }
}
