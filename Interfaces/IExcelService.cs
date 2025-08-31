using IdentityCardGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCardGenerator.Interfaces
{
    public interface IExcelService
    {
        Task<List<IdentityCard>> ReadIdentityCardsFromExcelAsync(string filePath);
    }
}
