using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCardGenerator.Models
{
    public class TemplateData
    {
        public required string CompanyName { get; set; }
        public required string HeaderText { get; set; }
        public required string FooterText { get; set; }
        public required string BackgroundColor { get; set; }
        public required string TextColor { get; set; }
        public required string BorderColor { get; set; }
        public bool ShowBarcode { get; set; }
        public bool ShowPhoto { get; set; }
        public bool ShowSignatureLine { get; set; }
    }
}
