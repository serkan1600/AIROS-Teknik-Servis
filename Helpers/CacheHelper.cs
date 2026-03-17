using System;
using System.Collections.Generic;
using System.Web;

namespace AIROSWEB.Helpers
{
    public static class CacheHelper
    {
        public static void ClearLayoutCache()
        {
            try
            {
                var cacheItems = HttpRuntime.Cache.GetEnumerator();
                var keysToRemove = new List<string>();
                while (cacheItems.MoveNext())
                {
                    string key = cacheItems.Key.ToString();
                    if (key.StartsWith("LayoutModel_"))
                    {
                        keysToRemove.Add(key);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    HttpRuntime.Cache.Remove(key);
                }
            }
            catch { }
        }
    }
}
