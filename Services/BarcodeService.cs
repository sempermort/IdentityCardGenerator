using IdentityCardGenerator.Interfaces;
using SkiaSharp;
using ZXing;
using ZXing.Common;


namespace IdentityCardGenerator.Services
{
   

public class BarcodeService : IBarcodeService
    {
        public Task<ImageSource> GenerateBarcodeAsync(string value, BarcodeFormat format, int width, int height)
        {
            return Task.Run(() =>
            {
                var writer = new ZXing.SkiaSharp.BarcodeWriter
                {
                    Format = format,
                    Options = new EncodingOptions
                    {
                        Width = width,
                        Height = height,
                        Margin = 0,
                        PureBarcode = true
                    },
                    Renderer = new ZXing.SkiaSharp.Rendering.SKBitmapRenderer()
                };

                using var bitmap = writer.Write(value);

                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return ImageSource.FromStream(() => data.AsStream());
            });
        }

        public async Task<string> SaveBarcodeAsync(string value, BarcodeFormat format, int width, int height, string filePath)
        {
            var imageSource = await GenerateBarcodeAsync(value, format, width, height);

            var writer = new ZXing.SkiaSharp.BarcodeWriter
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 0,
                    PureBarcode = true
                },
                Renderer = new ZXing.SkiaSharp.Rendering.SKBitmapRenderer()
            };

            using var bitmap = writer.Write(value);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);

            return filePath;
        }
    }


}