using System;
using System.Collections.Generic;
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
        private readonly string[] _imageFileTypes = {"jpg", "jpeg", "png", "gif"};
        private readonly string[] _sourceListFileTypes = {"csv"};
        private readonly Random _rng;
        private readonly StringBuilder _sb;

        public ImageProvider(IOptions<ConnectionStrings> options)
        {
            _filePath = options.Value.ImagesFolder;
            _rng = new Random();
            _sb = new StringBuilder();
        }

        public Task<string> GetRandomImageSource()
        {
            var files = new List<string>();
            var sourceLists = new List<string>();

            async Task<string> FormatAsDataUri(string filePath)
            {
                _sb.Append("data:image/")
                    .Append((Path.GetExtension(filePath) ?? "png").Replace(".", ""))
                    .Append(";base64,")
                    .Append(Convert.ToBase64String(await File.ReadAllBytesAsync(filePath)));
                
                return _sb.ToString();
            }

            foreach (var file in Directory.EnumerateFiles(_filePath, "*", SearchOption.AllDirectories))
            {
                switch (file)
                {
                    case var image when _imageFileTypes.Any(file.EndsWith):
                        files.Add(image);
                        break;
                    case var sourceList when _sourceListFileTypes.Any(file.EndsWith):
                        sourceLists.Add(sourceList);
                        break;
                }
            }

            var randomSource = files.Concat(sourceLists).ToList().GetRandomItemFromList(_rng);

            return sourceLists.Contains(randomSource) ? Task.FromResult(randomSource) : FormatAsDataUri(randomSource);
        }
    }
}
