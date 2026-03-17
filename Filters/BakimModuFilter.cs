using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using AIROSWEB.DAL.Db;

namespace AIROSWEB.Filters
{
    public class BakimModuFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Admin panelindeyse bakıma takılmasın
            if (filterContext.RouteData.DataTokens["area"]?.ToString() == "Admin")
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            try
            {
                // Önbellekten kontrol et
                bool? isMaintenanceMode = System.Web.HttpRuntime.Cache["BakimModu"] as bool?;
                
                if (isMaintenanceMode == null)
                {
                    using (var db = new AirosTeknikServisEntities())
                    {
                        var ayar = db.TblAyarlar.AsNoTracking().FirstOrDefault();
                        isMaintenanceMode = ayar?.BakimModu ?? false;
                        
                        // 1 dakika boyunca önbelleğe al (Hızlı güncelleme için kısa süre)
                        System.Web.HttpRuntime.Cache.Insert("BakimModu", isMaintenanceMode.Value, null, 
                            System.DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                }

                if (isMaintenanceMode == true)
                {
                    // Eğer kullanıcı yetkili bir admin ise bakımı görmesin
                    if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.Session?["AdminLogin"] != null)
                    {
                        base.OnActionExecuting(filterContext);
                        return;
                    }

                    // Bakım sayfasına yönlendir (eğer zaten bakım sayfasında değilse)
                    string controller = filterContext.RouteData.Values["controller"].ToString();
                    if (controller != "Maintenance")
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Maintenance", action = "Index" }));
                        return;
                    }
                }
            }
            catch
            {
                // Veritabanı sütunları henüz eklenmemiş olabilir (Setup çalıştırılmamış olabilir)
                // Bu durumda hatayı yutup siteyi çalıştırmaya devam et
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
