using ClosedXML.Excel;
using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Models;

namespace IdentityCardGenerator.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IBarcodeService _barcodeService;
        private readonly IPhotoService _photoService;

        public ExcelService(IBarcodeService barcodeService, IPhotoService photoService)
        {
            _barcodeService = barcodeService;
            _photoService = photoService;
        }
        public async Task<List<IdentityCard>> ReadIdentityCardsFromExcelAsync(string filePath)
        {
            var identityCards = new List<IdentityCard>();

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // First worksheet
                    var rows = worksheet.RowsUsed();

                    // Skip header row (assuming first row is header)
                    foreach (var row in rows.Skip(1))
                    {
                        var identityCard = new IdentityCard
                        {
                            FirstName = row.Cell(1).GetString().Trim(),
                            LastName = row.Cell(2).GetString().Trim(),
                            IdNumber = row.Cell(3).GetString().Trim(),
                            Department = row.Cell(4).GetString().Trim(),
                            Phone = row.Cell(5).GetString().Trim()   // must be full or relative path
                        };
                        

                        if (!string.IsNullOrWhiteSpace(identityCard.IdNumber))
                        {
                            // --- Resolve PhotoPath via PhotoService ---
                            string photoDir = Path.Combine(Path.GetDirectoryName(filePath)!, "Photos");
                            Directory.CreateDirectory(photoDir);
                            identityCard.PhotoPath = _photoService.GetPhotoPathForId(row.Cell(3).GetString().Trim(), photoDir);

                            string barcodesDir = Path.Combine(Path.GetDirectoryName(filePath)!, "Barcodes");
                            Directory.CreateDirectory(barcodesDir);

                            string barcodeFile = Path.Combine(barcodesDir, $"{identityCard.IdNumber}.png");

                            // Call your BarcodeService
                            identityCard.BarcodePath = await _barcodeService.SaveBarcodeAsync(
                                                            identityCard.IdNumber,
                                                            ZXing.BarcodeFormat.CODE_128, // or QR_CODE
                                                            300,
                                                            100,
                                                            barcodeFile
                            );
                        }

                        identityCards.Add(identityCard);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading Excel file: {ex.Message}", ex);
            }

            return await Task.FromResult(identityCards);
        }
    }
}
