using System;
using System.IO;
using SkiaSharp;

namespace MyDigimal.Common.Images
{
    public class ImageProcessor : IImageProcessor
    {
        public MemoryStream Base64ToImageStream(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return null;

            var imageBytes = Convert.FromBase64String(base64String);
            using var skBitmap = SKBitmap.Decode(imageBytes);
            return BitmapToStream(skBitmap);
        }

        public MemoryStream Base64ToThumbnailStream(string base64String, double maxHeight = 250, double maxWidth = 250)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            using var skBitmap = SKBitmap.Decode(imageBytes);

            var ratioX = maxWidth / skBitmap.Width;
            var ratioY = maxHeight / skBitmap.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(skBitmap.Width * ratio);
            var newHeight = (int)(skBitmap.Height * ratio);

            using var resizedBitmap = skBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
            return BitmapToStream(resizedBitmap);
        }

        private MemoryStream BitmapToStream(SKBitmap bitmap)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            var stream = new MemoryStream();
            data.SaveTo(stream);
            stream.Position = 0;
            return stream;
        }
    }
}