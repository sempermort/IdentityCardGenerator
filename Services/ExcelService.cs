using ClosedXML.Excel;
using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Models;

namespace IdentityCardGenerator.Services
{
    public class ExcelService:IExcelService
    {
        public Task<List<IdentityCard>> ReadIdentityCardsFromExcelAsync(string filePath)
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
                            Name = row.Cell(1).Value.ToString(),
                            IdNumber = row.Cell(2).Value.ToString()
                        };
                        identityCards.Add(identityCard);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle as appropriate for your application
                throw new Exception($"Error reading Excel file: {ex.Message}", ex);
            }

            return Task.FromResult(identityCards);
        }
    }
}