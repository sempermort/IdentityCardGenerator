using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCardGenerator.Models
{
    public class TemplateData
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string IdNumber { get; set; }
        public required string Department { get; set; }
        public string? Phone { get; set; }
        public required string PhotoPath { get; set; }
        public  string? BarcodePath { get; set; }
        public bool ShowBarcode { get; set; }
        public bool ShowPhoto { get; set; }
        public bool ShowSignatureLine { get; set; }
    }
}
