using IdentityCardGenerator.Interfaces;
using System;
using ZXing;
using ZXing.Common;

namespace IdentityCardGenerator.Services
{
    public class BarcodeService: IBarcodeService
    {
        public byte[] GenerateBarcode(string idNumber)
        {
            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Width = 300,
                        Height = 100,
                        Margin = 10
                    }
                };

                var pixelData = writer.Write(idNumber);
                return pixelData.Pixels;
            }
            catch (Exception ex)
            {
                // Log exception or handle as appropriate for your application
                throw new Exception($"Error generating barcode: {ex.Message}", ex);
            }
        }
    }
}