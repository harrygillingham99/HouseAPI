using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using House.HLL.Helpers;
using House.HLL.Images.Interfaces;
using Microsoft.Extensions.Options;

namespace House.HLL.Images
{
    public class ImageProvider : IImageProvider
    {
        private readonly string _filePath;
        private readonly string[] _imageFileTypes = {"jpg", "jpeg", "png"};
        private readonly Random _rng;
        private readonly StringBuilder _sb;

        public ImageProvider(IOptions<ConnectionStrings> options)
        {
            _filePath = options.Value.ImagesFolder;
            _rng = new Random();
            _sb = new StringBuilder();
        }

        public Task<string> GetRandomImageDataUri()
        {
            async Task<string> FormatAsDataUri(string filePath)
            {
                _sb.Append("data:image/")
                    .Append((Path.GetExtension(filePath) ?? "png").Replace(".", ""))
                    .Append(";base64,")
                    .Append(Convert.ToBase64String(await File.ReadAllBytesAsync(filePath)));
                
                return _sb.ToString();
            }

            var files = Directory.EnumerateFiles(_filePath, "*",
                SearchOption.AllDirectories).Where(file => _imageFileTypes.Any(file.EndsWith)).ToList();

            return FormatAsDataUri(files.GetRandomItemFromList(_rng));
        }
    }
}
