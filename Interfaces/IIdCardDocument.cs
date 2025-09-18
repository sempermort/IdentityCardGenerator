using IdentityCardGenerator.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCardGenerator.Interfaces
{
    public interface IIdCardDocument
    {

         void SaveAllAsPdf(ObservableCollection<IdentityCard> records, string filePath);

         byte[] CreateHexagonPhoto(string photoPath, int size);
       
    }
}
