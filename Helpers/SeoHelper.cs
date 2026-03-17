using System;
using System.Text.RegularExpressions;

namespace AIROSWEB.Helpers
{
    public static class SeoHelper
    {
        public static string ToSeoUrl(this string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            text = text.ToLower();
            text = text.Replace("ş", "s").Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ö", "o").Replace("ç", "c");
            text = Regex.Replace(text, @"[^a-z0-9\s-]", ""); // Remove invalid chars
            text = Regex.Replace(text, @"\s+", " ").Trim(); // Formatting
            text = Regex.Replace(text, @"\s", "-"); // Replace spaces
            return text;
        }
        public static string ToWhatsAppNumber(this string phone)
        {
            if (string.IsNullOrEmpty(phone)) return "";
            // Sadece rakamları al
            var clean = Regex.Replace(phone, @"[^\d]", "");
            // Eğer 0 ile başlıyorsa 0'ı at
            if (clean.StartsWith("0")) clean = clean.Substring(1);
            // Eğer 90 ile başlamıyorsa ve 10 haneli ise 90 ekle
            if (!clean.StartsWith("90") && clean.Length == 10) clean = "90" + clean;
            return clean;
        }
    }
}
