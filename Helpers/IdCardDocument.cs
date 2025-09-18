using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SkiaSharp;
using System.Collections.ObjectModel;


namespace IdentityCardGenerator.Helpers
{

    public class IdCardDocument :IIdCardDocument
    {

        public void SaveAllAsPdf(ObservableCollection<IdentityCard> records, string filePath)
        {
            // Pre-generate hexagon images (so expensive drawing happens once)
            var hexImages = new Dictionary<IdentityCard, byte[]>();
            foreach (var r in records)
                hexImages[r] = CreateHexagonPhoto(r.PhotoPath, 220);

            // Build document: one container.Page per record => one page per card
            Document.Create(container =>
            {
                foreach (var record in records)
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A6);
                        page.Margin(10);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Content().Padding(8).Column(col =>
                        {
                            // Photo (hexagon)
                            col.Item().AlignCenter().Element(e =>
                            {
                                var bytes = hexImages[record];
                                e.Width(220)
                                 .Height(220)
                                 .Image(new MemoryStream(bytes))
                                 .FitArea();
                            });

                            // Name
                            col.Item().AlignCenter().Text($"{record.FirstName} {record.LastName}")
                                .FontSize(20).Bold().FontColor(QuestPDF.Helpers.Colors.Black);

                            // Info table
                            col.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(90);
                                    c.RelativeColumn();
                                });

                                void AddRow(string label, string value)
                                {
                                    table.Cell().Text(label).Bold();
                                    table.Cell().Text(value ?? "");
                                }

                                AddRow("ID Number:", record.IdNumber);
                                AddRow("Department:", record.Department);
                                AddRow("Phone:", record.Phone);
                            });

                            // Barcode / QR
                            if(File.Exists(record.BarcodePath)) 
                                 col.Item().AlignCenter().PaddingTop(10).Image(record.BarcodePath);
                        });
                    });
                }
            })
            .GeneratePdf(filePath);
        }

        // --- SkiaSharp helper: create hexagon + clipped photo PNG ---
        public  byte[] CreateHexagonPhoto(string photoPath, int size = 220)
        {
            // Create SKBitmap to draw into
            using var bitmap = new SKBitmap(size, size, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Transparent);

            float cx = size / 2f;
            float cy = size / 2f;
            float radius = size * 0.45f;

            using var path = new SKPath();
            for (int i = 0; i < 6; i++)
            {
                // rotate so first point is top
                double angle = Math.PI / 3 * i - Math.PI / 2;
                float x = cx + (float)(radius * Math.Cos(angle));
                float y = cy + (float)(radius * Math.Sin(angle));
                if (i == 0) path.MoveTo(x, y); else path.LineTo(x, y);
            }
            path.Close();

            // Fill background hexagon (decorative color)
            using (var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = new SKColor(0x09, 0x3C, 0x07) })
                canvas.DrawPath(path, fill);

            // Draw photo clipped to the hexagon
            if (File.Exists(photoPath))
            {
                using var photo = SKBitmap.Decode(photoPath);
                if (photo != null)
                {
                    canvas.Save();
                    canvas.ClipPath(path, SKClipOperation.Intersect, true);

                    // center-crop the photo to fill the hexagon area
                    float scale = Math.Max((float)size / photo.Width, (float)size / photo.Height);
                    float w = photo.Width * scale;
                    float h = photo.Height * scale;
                    float left = (size - w) / 2f;
                    float top = (size - h) / 2f;
                    var dest = SKRect.Create(left, top, w, h);

                    canvas.DrawBitmap(photo, dest);
                    canvas.Restore();
                }
            }

            // Hexagon border (white outline)
            using (var stroke = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = SKColors.White, StrokeWidth = 4 })
                canvas.DrawPath(path, stroke);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

       
    }
}