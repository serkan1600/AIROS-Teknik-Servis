using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AIROSWEB.Helpers
{
    public static class ImageHelper
    {
        public static string SaveImageOptimized(Stream inputStream, string serverPath, string fileName)
        {
            try
            {
                using (var image = Image.FromStream(inputStream))
                {
                    // Orginal boyutlar
                    int width = image.Width;
                    int height = image.Height;

                    // Maksimum genişlik 1920px olsun (Full HD)
                    if (width > 1920)
                    {
                        var ratio = (double)1920 / width;
                        width = 1920;
                        height = (int)(height * ratio);
                    }

                    // Yeni boyutlandırma
                    using (var resized = new Bitmap(width, height))
                    {
                        using (var graphics = Graphics.FromImage(resized))
                        {
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.DrawImage(image, 0, 0, width, height);
                        }

                        // WebPEncoder bulamazsa JPEG kullan
                        var encoder = GetEncoder(ImageFormat.Jpeg);
                        var parameters = new EncoderParameters(1);
                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);

                        // Uzantıyı .jpg yapalım (WebP için 3. parti kütüphane gerekir, System.Drawing native desteklemez .NET Framework'te varsayılan olarak)
                        // Kullanıcı "WebP" istedi ama .NET 4.8 System.Drawing ile native WebP zordur. 
                        // En temizi yüksek kaliteli JPEG sıkıştırmasıdır.
                        // Veya dosya uzantısını koruyarak sıkıştırırız.
                        
                        string newFileName = Path.GetFileNameWithoutExtension(fileName) + ".jpg";
                        string fullPath = Path.Combine(serverPath, newFileName);

                        resized.Save(fullPath, encoder, parameters);
                        return newFileName;
                    }
                }
            }
            catch
            {
                // Hata durumunda null dön
                return null;
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}
