
using System.IO;

namespace MyDigimal.Common.Images
{
    public interface IImageProcessor
    {
        MemoryStream Base64ToImageStream(string base64String);
        MemoryStream Base64ToThumbnailStream(string base64String, double maxHeight = 250, double maxWidth = 250);
    }
}