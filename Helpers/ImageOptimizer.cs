using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace AIROSWEB.Helpers
{
    public static class ImageOptimizer
    {
        /// <summary>
        /// Resmi optimize eder ve sıkıştırır
        /// </summary>
        public static void OptimizeImage(string filePath, int maxWidth = 1920, int quality = 85)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                
                // Sadece büyük dosyaları optimize et (500 KB üzeri)
                if (fileInfo.Length < 500 * 1024)
                    return;

                using (var image = Image.FromFile(filePath))
                {
                    // Boyutu kontrol et
                    int newWidth = image.Width;
                    int newHeight = image.Height;

                    if (image.Width > maxWidth)
                    {
                        newWidth = maxWidth;
                        newHeight = (int)((double)image.Height / image.Width * maxWidth);
                    }

                    // Yeniden boyutlandır
                    using (var newImage = new Bitmap(newWidth, newHeight))
                    {
                        using (var graphics = Graphics.FromImage(newImage))
                        {
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                        }

                        // JPEG encoder ile kaydet
                        var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                        // Geçici dosyaya kaydet
                        var tempFile = filePath + ".tmp";
                        newImage.Save(tempFile, jpegEncoder, encoderParameters);

                        // Orijinal dosyayı sil ve yenisiyle değiştir
                        File.Delete(filePath);
                        File.Move(tempFile, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata logla ama devam et
                System.Diagnostics.Debug.WriteLine($"Image optimization error: {ex.Message}");
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        /// <summary>
        /// Klasördeki tüm resimleri optimize et
        /// </summary>
        public static int OptimizeFolder(string folderPath, int maxWidth = 1920, int quality = 85)
        {
            int optimizedCount = 0;
            var extensions = new[] { ".jpg", ".jpeg", ".png" };
            
            var files = Directory.GetFiles(folderPath)
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                .ToList();

            foreach (var file in files)
            {
                OptimizeImage(file, maxWidth, quality);
                optimizedCount++;
            }

            return optimizedCount;
        }
    }
}
