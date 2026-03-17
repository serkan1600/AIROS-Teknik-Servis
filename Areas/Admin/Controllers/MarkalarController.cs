using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AIROSWEB.BLL.Managers;
using AIROSWEB.DAL.Db;
using AIROSWEB.Helpers;

namespace AIROSWEB.Areas.Admin.Controllers
{
    [Authorize]
    public class MarkalarController : Controller
    {
        MarkaManager manager = new MarkaManager();

        public ActionResult Index()
        {
            var values = manager.TumMarkalariGetir();
            return View(values);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TblMarkalar p)
        {
            p.AktifMi = true;
            manager.MarkaEkle(p);
            CacheHelper.ClearLayoutCache();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            var value = manager.MarkaBul(id.Value);
            if (value == null) return RedirectToAction("Index");
            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TblMarkalar p)
        {
            if (ModelState.IsValid)
            {
                var value = manager.MarkaBul(p.Id);
                if (value != null)
                {
                    value.LogoYolu = p.LogoYolu;
                    value.MarkaAdi = p.MarkaAdi;
                    value.AktifMi = p.AktifMi;
                    manager.MarkaGuncelle(value);
                    CacheHelper.ClearLayoutCache();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            manager.MarkaSil(id);
            CacheHelper.ClearLayoutCache();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FixAll()
        {
            string[] standartMarkalar = { "Bosch", "Vaillant", "Demirdöküm", "Baymak", "ECA", "Viessmann", "Buderus", "Ariston", "Copa" };
            var mevcutlar = manager.TumMarkalariGetir();

            foreach (var markaAd in standartMarkalar)
            {
                var marka = mevcutlar.FirstOrDefault(x => x.MarkaAdi.ToLower().Contains(markaAd.ToLower()));
                if (marka == null)
                {
                    marka = new TblMarkalar { MarkaAdi = markaAd, AktifMi = true };
                    manager.MarkaEkle(marka);
                }
                marka.AktifMi = true;
                string ad = marka.MarkaAdi.ToLower();
                if (ad.Contains("bosch")) marka.LogoYolu = "/Uploads/Markalar/bosch.png";
                else if (ad.Contains("vaillant")) marka.LogoYolu = "/Uploads/Markalar/vaillant.png";
                else if (ad.Contains("demirdöküm") || ad.Contains("demirdokum")) marka.LogoYolu = "/Uploads/Markalar/demirdokum.png";
                else if (ad.Contains("baymak")) marka.LogoYolu = "/Uploads/Markalar/baymak.png";
                else if (ad.Contains("eca")) marka.LogoYolu = "/Uploads/Markalar/eca.png";
                else if (ad.Contains("viessmann")) marka.LogoYolu = "/Uploads/Markalar/viessmann.png";
                else if (ad.Contains("buderus")) marka.LogoYolu = "/Uploads/Markalar/buderus.png";
                else if (ad.Contains("ariston")) marka.LogoYolu = "/Uploads/Markalar/ariston.png";
                else if (ad.Contains("copa")) marka.LogoYolu = "/Uploads/Markalar/copa.png";

                manager.MarkaGuncelle(marka);
            }
            return RedirectToAction("Index");
        }
    }
}
