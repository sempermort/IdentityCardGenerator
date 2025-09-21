
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SkiaSharp;
using IdentityCardGenerator.Models;
using System.Collections.ObjectModel;
using System.IO;
using IdentityCardGenerator.Interfaces;

namespace IdentityCardGenerator.Services
{
    public class IdCardDocument : IIdCardDocument
    {
        // ID card dimensions
        private const double CardWidth = 250;
        private const double CardHeight = 150;
        private const double Margin = 20;

        private static readonly byte[] PlaceholderPng = new byte[]
        {
            0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A,
            0x00,0x00,0x00,0x0D,0x49,0x48,0x44,0x52,
            0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x01,
            0x08,0x02,0x00,0x00,0x00,0x90,0x77,0x53,
            0xDE,0x00,0x00,0x00,0x0A,0x49,0x44,0x41,
            0x54,0x08,0xD7,0x63,0xF8,0xCF,0xC0,0x00,
            0x00,0x03,0x01,0x01,0x00,0x18,0xDD,0x8D,
            0xB1,0x00,0x00,0x00,0x00,0x49,0x45,0x4E,
            0x44,0xAE,0x42,0x60,0x82
        };

        public void SaveAllAsPdf(ObservableCollection<IdentityCard> records, string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                folderPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedIds");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"IDCardsSheet_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(folderPath, fileName);

            var doc = new PdfDocument();
            var page = doc.AddPage();
            page.Width = XUnit.FromMillimeter(210); // A4 width
            page.Height = XUnit.FromMillimeter(297); // A4 height

            var gfx = XGraphics.FromPdfPage(page);

            double x = Margin;
            double y = Margin;
            int cardsPerRow = (int)((page.Width.Point - Margin) / (CardWidth + Margin));
            int cardCount = 0;

            foreach (var record in records)
            {
                DrawIdCard(gfx, x, y, record);

                // Next position
                cardCount++;
                if (cardCount % cardsPerRow == 0)
                {
                    x = Margin;
                    y += CardHeight + Margin;
                    if (y + CardHeight + Margin > page.Height.Point)
                    {
                        page = doc.AddPage();
                        page.Width = XUnit.FromMillimeter(210);
                        page.Height = XUnit.FromMillimeter(297);
                        gfx = XGraphics.FromPdfPage(page);
                        x = Margin;
                        y = Margin;
                    }
                }
                else
                {
                    x += CardWidth + Margin;
                }
            }

            doc.Save(filePath);
            doc.Close();
            Console.WriteLine($"PDF sheet generated: {filePath}");
        }

        private void DrawIdCard(XGraphics gfx, double x, double y, IdentityCard record)
        {
            // ---------------------
            // Rotated background borders
            DrawRotatedRectangle(gfx, x + 0, y + 0, 480, 450, -55, 80, -276, XColor.FromArgb(0x1b, 0x46, 0x10), XColors.Black, 2);
            DrawRotatedRectangle(gfx, x + 0, y + 0, 240, 390, -63, -90, -179, XColor.FromArgb(0x58, 0x99, 0x35), XColors.Black, 2);

            // Draw border around the card
            gfx.DrawRectangle(XPens.Black, x, y, CardWidth, CardHeight);

            // ---------------------
            // Photo
            byte[] photoBytes = File.Exists(record.PhotoPath)
                ? CreateHexagonPhotoFromFile(record.PhotoPath, 80)
                : CreateHexagonPhotoFromBytes(PlaceholderPng, 80);

            using (var ms = new MemoryStream(photoBytes))
            {
                var img = XImage.FromStream(() => ms);
                gfx.DrawImage(img, x + (CardWidth - 80) / 2, y + 10, 80, 80);
            }

            // ---------------------
            // Name
            gfx.DrawString($"{record.FirstName} {record.LastName}",
                new XFont("Arial", 12, XFontStyle.Bold),
                XBrushes.Black,
                new XRect(x, y + 95, CardWidth, 15),
                XStringFormats.TopCenter);

            // Department + phone
            gfx.DrawString($"Dept: {record.Department ?? ""}",
                new XFont("Arial", 10),
                XBrushes.Black,
                new XRect(x + 10, y + 115, CardWidth - 20, 12),
                XStringFormats.TopLeft);

            gfx.DrawString($"Phone: {record.Phone ?? ""}",
                new XFont("Arial", 10),
                XBrushes.Black,
                new XRect(x + 10, y + 130, CardWidth - 20, 12),
                XStringFormats.TopLeft);

            // Barcode
            if (!string.IsNullOrEmpty(record.BarcodePath) && File.Exists(record.BarcodePath))
            {
                byte[] barcodeBytes = File.ReadAllBytes(record.BarcodePath);
                var barcodeImg = XImage.FromStream(() => new MemoryStream(barcodeBytes));
                gfx.DrawImage(barcodeImg, x + (CardWidth - 100) / 2, y + 145, 100, 25);
            }
        }

        // ---------------------
        private void DrawRotatedRectangle(XGraphics gfx, double originX, double originY, double width, double height,
            double rotationDegrees, double translationX, double translationY,
            XColor backgroundColor, XColor borderColor, double strokeThickness)
        {
            gfx.Save();
            gfx.TranslateTransform(originX + translationX, originY + translationY);
            gfx.RotateTransform(rotationDegrees);
            gfx.DrawRectangle(new XSolidBrush(backgroundColor), 0, 0, width, height);
            gfx.DrawRectangle(new XPen(borderColor, strokeThickness), 0, 0, width, height);
            gfx.Restore();
        }

        // ---------------------
        public byte[] CreateHexagonPhotoFromFile(string filePath, int size = 80)
        {
            try
            {
                using var photo = File.OpenRead(filePath);
                return CreateHexagonPhotoFromBytes(ReadAllBytes(photo), size);
            }
            catch
            {
                return CreateHexagonPhotoFromBytes(PlaceholderPng, size);
            }
        }

        private byte[] CreateHexagonPhotoFromBytes(byte[] imgBytes, int size = 80)
        {
            using var bitmap = new SKBitmap(size, size);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Transparent);

            float cx = size / 2f;
            float cy = size / 2f;
            float radius = size * 0.45f;

            using var path = new SKPath();
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i - Math.PI / 2;
                float px = cx + (float)(radius * Math.Cos(angle));
                float py = cy + (float)(radius * Math.Sin(angle));
                if (i == 0) path.MoveTo(px, py); else path.LineTo(px, py);
            }
            path.Close();

            using (var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = new SKColor(0x09, 0x3C, 0x07) })
                canvas.DrawPath(path, fill);

            using var photo = SKBitmap.Decode(imgBytes);
            if (photo != null)
            {
                canvas.Save();
                canvas.ClipPath(path, SKClipOperation.Intersect, true);

                float scale = Math.Max((float)size / photo.Width, (float)size / photo.Height);
                float w = photo.Width * scale;
                float h = photo.Height * scale;
                float left = (size - w) / 2f;
                float top = (size - h) / 2f;
                canvas.DrawBitmap(photo, SKRect.Create(left, top, w, h));
                canvas.Restore();
            }

            using (var stroke = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = SKColors.White, StrokeWidth = 2 })
                canvas.DrawPath(path, stroke);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        private byte[] ReadAllBytes(Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
