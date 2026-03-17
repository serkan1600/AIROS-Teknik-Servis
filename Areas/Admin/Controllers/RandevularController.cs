using System;
using System.Linq;
using System.Web.Mvc;
using AIROSWEB.DAL.Db;
using System.Data.Entity;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class RandevularController : Controller
    {
        AirosTeknikServisEntities db = new AirosTeknikServisEntities();

        // GET: Admin/Randevular
        public ActionResult Index(string durum = "")
        {
            var query = db.TblRandevular.AsQueryable();
            
            // Eğer giriş yapan kullanıcı Teknisyense sadece kendi randevularını görsün
            var rol = Session["Rol"] as string;
            if (rol == "Teknisyen")
            {
                int myId = Convert.ToInt32(Session["AdminId"]);
                query = query.Where(x => x.TeknisyenId == myId);
            }

            if (!string.IsNullOrEmpty(durum))
            {
                query = query.Where(x => x.Durum == durum);
            }

            var randevular = query.OrderByDescending(x => x.OlusturmaTarihi).ToList();
            return View(randevular);
        }

        // GET: Admin/Randevular/Details/5
        public ActionResult Details(int id)
        {
            var randevu = db.TblRandevular.Find(id);
            if (randevu == null) return HttpNotFound();
            
            // Teknisyenlerin Listesi (Admins with Role="Teknisyen")
            ViewBag.Teknisyenler = db.TblAdmin.Where(x => x.Rol == "Teknisyen" && x.AktifMi == true).ToList();
            
            return View(randevu);
        }

        // POST: Admin/Randevular/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateStatus(int id, string durum, string not, string alinanUcret)
        {
            var randevu = db.TblRandevular.Find(id);
            if (randevu != null)
            {
                string eskiDurum = randevu.Durum;
                randevu.Durum = durum;
                randevu.YoneticiNotu = not;
                
                if (!string.IsNullOrEmpty(alinanUcret))
                {
                    randevu.AlinanUcret = alinanUcret;
                }
                
                db.SaveChanges();
                TempData["Success"] = "Randevu durumu güncellendi.";

                // E-Mail Bildirimi
                if (eskiDurum != durum && (durum == "Onaylandı" || durum == "Tamamlandı") && !string.IsNullOrEmpty(randevu.Email))
                {
                    string body = "";
                    string subject = "";
                    string formatliTarih = randevu.RandevuTarihi.HasValue ? randevu.RandevuTarihi.Value.ToString("dd.MM.yyyy") : "Belirtilmedi";
                    string hizAdi = string.IsNullOrEmpty(randevu.HizmetAdi) ? "hizmet" : randevu.HizmetAdi;

                    if (durum == "Onaylandı")
                    {
                        subject = "Randevunuz Onaylandı";
                        body = $"Sayın {randevu.AdSoyad},<br><br>Randevunuz onaylanmıştır.<br>Tarih: {formatliTarih} Saat: {randevu.RandevuSaati}<br>Not: {not}<br><br>Teşekkürler, AIROS Teknik Servis";
                    }
                    else if (durum == "Tamamlandı")
                    {
                        subject = "İşleminiz Tamamlandı";
                        body = $"Sayın {randevu.AdSoyad},<br><br>Talep etmiş olduğunuz işlem ({hizAdi}) başarıyla tamamlanmıştır.<br>Bizi tercih ettiğiniz için teşekkür ederiz.<br><br>Teşekkürler, AIROS Teknik Servis";
                    }
                    
                    AIROSWEB.Helpers.MailHelper.SendMail(randevu.Email, subject, body);
                }
            }
            return RedirectToAction("Details", new { id = id });
        }

        // POST: Admin/Randevular/TeknisyenAta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeknisyenAta(int randevuId, int? teknisyenId)
        {
            var randevu = db.TblRandevular.Find(randevuId);
            if (randevu != null && Session["Rol"] != null && Session["Rol"].ToString() == "Admin")
            {
                randevu.TeknisyenId = teknisyenId;
                if (teknisyenId.HasValue)
                {
                    randevu.Durum = "Onaylandı"; // Atanınca otomatik Onaylandı olsun
                }
                db.SaveChanges();
                TempData["Success"] = "Randevu başarıyla teknisyene atandı.";
            }
            return RedirectToAction("Details", new { id = randevuId });
        }

        // POST: Admin/Randevular/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var randevu = db.TblRandevular.Find(id);
            if (randevu != null)
            {
                db.TblRandevular.Remove(randevu);
                db.SaveChanges();
                TempData["Success"] = "Randevu kaydı silindi.";
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Randevular/Edit/5
        public ActionResult Edit(int id)
        {
            var randevu = db.TblRandevular.Find(id);
            if (randevu == null) return HttpNotFound();
            
            ViewBag.Teknisyenler = db.TblAdmin.Where(x => x.Rol == "Teknisyen" && x.AktifMi == true).ToList();
            
            return View(randevu);
        }

        // POST: Admin/Randevular/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblRandevular model)
        {
            if (ModelState.IsValid)
            {
                var randevu = db.TblRandevular.Find(model.Id);
                if (randevu != null)
                {
                    string eskiDurum = randevu.Durum;
                    
                    randevu.AdSoyad = model.AdSoyad;
                    randevu.Telefon = model.Telefon;
                    randevu.HizmetAdi = model.HizmetAdi;
                    randevu.RandevuTarihi = model.RandevuTarihi;
                    randevu.RandevuSaati = model.RandevuSaati;
                    randevu.Adres = model.Adres;
                    randevu.Durum = model.Durum;
                    randevu.YoneticiNotu = model.YoneticiNotu;
                    randevu.Email = model.Email;
                    randevu.AlinanUcret = model.AlinanUcret;
                    randevu.TeknisyenId = model.TeknisyenId;
                    
                    db.SaveChanges();
                    TempData["Success"] = "Randevu bilgileri güncellendi.";

                    // E-Mail Bildirimi
                    if (eskiDurum != model.Durum && (model.Durum == "Onaylandı" || model.Durum == "Tamamlandı") && !string.IsNullOrEmpty(model.Email))
                    {
                        string body = "";
                        string subject = "";
                        string formatliTarih = model.RandevuTarihi.HasValue ? model.RandevuTarihi.Value.ToString("dd.MM.yyyy") : "Belirtilmedi";
                        string hizAdi = string.IsNullOrEmpty(model.HizmetAdi) ? "hizmet" : model.HizmetAdi;

                        if (model.Durum == "Onaylandı")
                        {
                            subject = "Randevunuz Onaylandı";
                            body = $"Sayın {model.AdSoyad},<br><br>Randevunuz onaylanmıştır.<br>Tarih: {formatliTarih} Saat: {model.RandevuSaati}<br>Not: {model.YoneticiNotu}<br><br>Teşekkürler, AIROS Teknik Servis";
                        }
                        else if (model.Durum == "Tamamlandı")
                        {
                            subject = "İşleminiz Tamamlandı";
                            body = $"Sayın {model.AdSoyad},<br><br>Talep etmiş olduğunuz işlem ({hizAdi}) başarıyla tamamlanmıştır.<br>Bizi tercih ettiğiniz için teşekkür ederiz.<br><br>Teşekkürler, AIROS Teknik Servis";
                        }
                        
                        AIROSWEB.Helpers.MailHelper.SendMail(model.Email, subject, body);
                    }

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
