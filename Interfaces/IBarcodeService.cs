using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace IdentityCardGenerator.Interfaces
{
    public interface IBarcodeService
    {
        Task<ImageSource> GenerateBarcodeAsync(string value, BarcodeFormat format, int width, int height);
        Task<string> SaveBarcodeAsync(string value, BarcodeFormat format, int width, int height, string filePath);
    }

}
