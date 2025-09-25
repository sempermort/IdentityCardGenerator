
using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Models;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using PointF = System.Drawing.PointF;

namespace IdentityCardGenerator.Services
{
    public class IdCardDocument : IIdCardDocument
    {
       
      



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
            page.Width = XUnit.FromMillimeter(210);  // A4 width
            page.Height = XUnit.FromMillimeter(297); // A4 height

            var gfx = XGraphics.FromPdfPage(page);

            double CardWidth = XUnit.FromMillimeter(60);   // ID card width
            double CardHeight = XUnit.FromMillimeter(90);  // ID card height
            double Margin = 15;       // spacing between cards

            // Dynamically calculate how many cards fit per row & column
            int cardsPerRow = (int)((page.Width.Point - Margin) / (CardWidth + Margin));
            int cardsPerColumn = (int)((page.Height.Point - Margin) / (CardHeight + Margin));

            double xStart = Margin;
            double yStart = Margin;
            double x = xStart;
            double y = yStart;

            int cardCount = 0;

            foreach (var record in records)
            {
                DrawIdCard(gfx, x, y, CardWidth, CardHeight, record);

                cardCount++;

                // Move to next column
                if (cardCount % cardsPerRow == 0)
                {
                    x = xStart;
                    y += CardHeight + Margin;

                    // New page if current page full
                    if ((cardCount / cardsPerRow) % cardsPerColumn == 0 && cardCount < records.Count)
                    {
                        page = doc.AddPage();
                        page.Width = XUnit.FromMillimeter(210);
                        page.Height = XUnit.FromMillimeter(297);
                        gfx = XGraphics.FromPdfPage(page);

                        x = xStart;
                        y = yStart;
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


        private void DrawIdCard(XGraphics gfx, double x, double y, double CardWidth, double CardHeight, IdentityCard record)
        {
            // --- Clip to card area so nothing spills outside ---
            gfx.Save();
            gfx.IntersectClip(new XRect(x, y, CardWidth, CardHeight));

            // --- Rotated background rectangles (smaller & centered) ---
            double centerX = x + CardWidth / 2;
            double centerY = y + CardHeight / 2;

            // Dark green rotated rectangle
            DrawRotatedRectangle(gfx, centerX, centerY, CardWidth * 1.2, CardHeight * 0.6, 30, -34.51, -259.836,
                XColor.FromArgb(0x1b, 0x46, 0x10), XColor.FromArgb(0x1b, 0x46, 0x10), 0, 15);

            // Light green rotated rectangle
            DrawRotatedRectangle(gfx, centerX, centerY, CardWidth * 1.2, CardHeight * 0.38, 30, -158.36, -192.38,
                XColor.FromArgb(0x81, 0xc3, 0x41), XColor.FromArgb(0x81, 0xc3, 0x41), 0, 15);

            gfx.Restore(); // remove clipping so other elements (like barcode) can draw freely

            // --- Card border ---
            gfx.DrawRectangle(XPens.Tan, x, y, CardWidth, CardHeight);

            // --- Top-right header & logo ---
            double headerX = x + CardWidth - 70;
            double headerY = y + 5;

            gfx.DrawString("MPM", new XFont("Poppins", 7, XFontStyle.Bold), XBrushes.White, new XRect(headerX, headerY, 100, 12), XStringFormats.CenterLeft);
            gfx.DrawString("KASULU SUGAR", new XFont("Poppins", 7, XFontStyle.Regular), XBrushes.White, new XRect(headerX, headerY + 12, 100, 12), XStringFormats.CenterLeft);

            string logoPath = Path.Combine(AppContext.BaseDirectory, "logo.png");
            if (File.Exists(logoPath))
            {
                byte[] logoBytes = File.ReadAllBytes(logoPath);
                using var msLogo = new MemoryStream(logoBytes);
                var logoImg = XImage.FromStream(() => msLogo);

                double logoWidth = 35;
                double logoHeight = 35;
                double logoX = x + CardWidth - logoWidth - 5;
                double logoY = headerY + 25;

                gfx.DrawImage(logoImg, logoX, logoY, logoWidth, logoHeight);
            }

            // --- Photo hexagon ---
            double photoSize = 85;
            double photoX = x + (CardWidth - photoSize) / 2;
            double photoY = y + 40;
            double cx = photoX + photoSize / 2;
            double cy = photoY + photoSize / 2;
            double cornerRadius = 20;

            DrawRoundedHexagon(gfx, cx, cy, (photoSize + 20) / 2,cornerRadius, new XSolidBrush(XColor.FromArgb(0x81, 0xC3, 0x41)), new XPen(XColors.Transparent, 0));
            DrawRoundedHexagon(gfx, cx, cy, (photoSize + 3) / 2, cornerRadius, new XSolidBrush(XColors.White), new XPen(XColors.Transparent, 0));

            byte[] photoHex = File.Exists(record.PhotoPath) ? CreateHexagonPhotoFromFile(record.PhotoPath, (int)photoSize) : CreateHexagonPhotoFromBytes(PlaceholderPng, (int)photoSize);
            using (var msPhoto = new MemoryStream(photoHex))
            {
                var img = XImage.FromStream(() => msPhoto);
                gfx.DrawImage(img, photoX, photoY, photoSize, photoSize);
            }

            // --- Texts stacked vertically ---
            double textStartY = photoY + photoSize + 8;
            double lineHeight = 18;

            gfx.DrawString($"{record.FirstName} {record.LastName}", new XFont("PoppinsBold", 12, XFontStyle.Bold), new XSolidBrush(XColor.FromArgb(0x23, 0x1F, 0x55)), new XRect(x, textStartY, CardWidth, lineHeight), XStringFormats.TopCenter);

            double roleY = textStartY + lineHeight + 3;
            double textWidth = CardWidth - 30;
            double textX = x + 15;
            double boxHeight = 20;

            gfx.DrawRoundedRectangle(new XPen(XColors.Transparent, 0), new XSolidBrush(XColor.FromArgb(0x58, 0x99, 0x35)), new XRect(textX, roleY, textWidth, boxHeight), new XSize(8, 8));
            gfx.DrawString(record.Position ?? "", new XFont("RobotoVariable", 9, XFontStyle.Bold), XBrushes.White, new XRect(textX, roleY, textWidth, boxHeight), XStringFormats.Center);

            double idY = roleY + 32;
            gfx.DrawString("ID No: ", new XFont("Poppins", 9, XFontStyle.Bold), XBrushes.Black, new XPoint(x + 10, idY));
            gfx.DrawString(record.IdNumber ?? "", new XFont("Poppins", 9, XFontStyle.Regular), new XSolidBrush(XColor.FromArgb(0x23, 0x1F, 0x55)), new XPoint(x + 55, idY));

            double deptY = idY + 18;
            gfx.DrawString("Dept: ", new XFont("Poppins", 9, XFontStyle.Bold), XBrushes.Black, new XPoint(x + 10, deptY));
            gfx.DrawString(record.Department ?? "", new XFont("Poppins", 9, XFontStyle.Regular), new XSolidBrush(XColor.FromArgb(0x23, 0x1F, 0x55)), new XPoint(x + 55, deptY));

            double phoneY = deptY + 18;
            gfx.DrawString("Phone: ", new XFont("Poppins", 9, XFontStyle.Bold), XBrushes.Black, new XPoint(x + 10, phoneY));
            gfx.DrawString(record.Phone ?? "", new XFont("Poppins", 9, XFontStyle.Regular), new XSolidBrush(XColor.FromArgb(0x23, 0x1F, 0x55)), new XPoint(x + 55, phoneY));

            if (!string.IsNullOrEmpty(record.BarcodePath) && File.Exists(record.BarcodePath))
            {
                double barcodeY = phoneY + 4;
                byte[] barcodeBytes = File.ReadAllBytes(record.BarcodePath);
                var barcodeImg = XImage.FromStream(() => new MemoryStream(barcodeBytes));
                gfx.DrawImage(barcodeImg, x + (CardWidth - 100) / 2, barcodeY, 100, 20);
            }
            gfx.Save();
            gfx.IntersectClip(new XRect(x, y, CardWidth, CardHeight));
            DrawRotatedRectangle(gfx, centerX, centerY, 326.44, 178.09, 37, -100,85, XColor.FromArgb(0x58, 0x99, 0x35), XColor.FromArgb(0x58, 0x99, 0x35), 0, 30);

            DrawRotatedRectangle(gfx, centerX, centerY, 325.07, 178.09, 30, -347, -43, XColor.FromArgb(0x1b, 0x46, 0x10), XColor.FromArgb(0x1b, 0x46, 0x10), 0, 30);
            gfx.Restore();
        }

        // ---------------------
        private void DrawRotatedRectangle(
           XGraphics gfx,
           double originX, double originY,
           double width, double height,
           double rotationDegrees,
           double translationX, double translationY,
           XColor backgroundColor,
           XColor borderColor,
           double strokeThickness,
           double cornerRadius = 20) // new param for roundness
        {
            gfx.Save();

            // Move and rotate
            gfx.TranslateTransform(originX + translationX, originY + translationY);
            gfx.RotateTransform(rotationDegrees);

            // Background fill with rounded corners
            gfx.DrawRoundedRectangle(
                new XSolidBrush(backgroundColor),
                0, 0, width, height,
                cornerRadius, cornerRadius);

            // Border with rounded corners
            gfx.DrawRoundedRectangle(
                new XPen(borderColor, strokeThickness),
                0, 0, width, height,
                cornerRadius, cornerRadius);

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

        private byte[] CreateHexagonPhotoFromBytes(byte[] imgBytes, int size = 85)
        {
            using var bitmap = new SKBitmap(size, size);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Transparent);

            float cx = size / 2f;
            float cy = size / 2f;
            float radius = size * 0.5f;

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
        private byte[] CreateHexagonShape(double size, XColor fillColor, XColor borderColor, double borderWidth)
        {
            int w = (int)size;
            int h = (int)size;

            using (var bmp = new Bitmap(w, h))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                PointF[] hexPoints = GetHexagonPoints(w, h);

                using (var brush = new SolidBrush(System.Drawing.Color.FromArgb((int)fillColor.A, fillColor.R, fillColor.G, fillColor.B)))
                {
                    g.FillPolygon(brush, hexPoints);
                }

                using (var pen = new Pen(System.Drawing.Color.FromArgb((int)borderColor.A, borderColor.R, borderColor.G, borderColor.B), (float)borderWidth))
                {
                    g.DrawPolygon(pen, hexPoints);
                }

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    return ms.ToArray();
                }
            }
        }
        private void DrawRoundedHexagon(XGraphics gfx,double centerX,double centerY,double size,double cornerRadius, XBrush fill ,XPen pen)    // pass null or XBrushes.Transparent for no fill
        {
            // 1) compute the 6 vertices (flat-top hexagon)
            XPoint[] verts = new XPoint[6];
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 180.0 * (60 * i - 30); // flat-top orientation
                verts[i] = new XPoint(centerX + size * Math.Cos(angle), centerY + size * Math.Sin(angle));
            }

            // 2) compute p1/p2 for each corner (cut-back points on incoming/outgoing edges)
            XPoint[] p1 = new XPoint[6];
            XPoint[] p2 = new XPoint[6];

            for (int i = 0; i < 6; i++)
            {
                XPoint prev = verts[(i + 5) % 6];
                XPoint curr = verts[i];
                XPoint next = verts[(i + 1) % 6];

                // vectors along edges: prev->curr and curr->next
                double dx0 = curr.X - prev.X;
                double dy0 = curr.Y - prev.Y;
                double dx1 = next.X - curr.X;
                double dy1 = next.Y - curr.Y;

                double len0 = Math.Sqrt(dx0 * dx0 + dy0 * dy0);
                double len1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);

                // avoid division by zero; if degenerate, fall back to small lengths
                if (len0 < 1e-6) len0 = 1e-6;
                if (len1 < 1e-6) len1 = 1e-6;

                // clamp cornerRadius so it doesn't exceed half the adjacent edge lengths
                double cr = Math.Min(cornerRadius, Math.Min(len0, len1) / 2.0);

                // unit directions
                double ux0 = dx0 / len0;
                double uy0 = dy0 / len0;
                double ux1 = dx1 / len1;
                double uy1 = dy1 / len1;

                // p1: point along previous edge before the corner
                p1[i] = new XPoint(curr.X - ux0 * cr, curr.Y - uy0 * cr);
                // p2: point along next edge after the corner
                p2[i] = new XPoint(curr.X + ux1 * cr, curr.Y + uy1 * cr);
            }

            // 3) build the path WITHOUT using GetLastPoint()
            var path = new XGraphicsPath();

            // start by connecting last corner's p2 to first corner's p1
            path.AddLine(p2[5], p1[0]);
            // curve around corner 0
            path.AddBezier(p1[0], verts[0], verts[0], p2[0]);

            // then for corners 1..5: connect previous p2 -> current p1, then curve to current p2
            for (int i = 1; i < 6; i++)
            {
                path.AddLine(p2[i - 1], p1[i]);
                path.AddBezier(p1[i], verts[i], verts[i], p2[i]);
            }

            path.CloseFigure();

            // 4) draw
            gfx.DrawPath(pen, fill ?? XBrushes.Transparent, path);
        }


        private PointF[] GetHexagonPoints(int width, int height)
        {
            float w = width;
            float h = height;
            float halfW = w / 2;
            float quarterW = w / 4;
            float threeQuarterW = 3 * w / 4;

            return new[]
            {
        new PointF(quarterW, 0),
        new PointF(threeQuarterW, 0),
        new PointF(w, h / 2),
        new PointF(threeQuarterW, h),
        new PointF(quarterW, h),
        new PointF(0, h / 2)
    };
        }

        private byte[] ReadAllBytes(Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
