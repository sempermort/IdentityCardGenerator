using IdentityCardGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCardGenerator.Interfaces
{
    public interface ITemplateService
    {
        Task SaveTemplateAsync(TemplateViewModel template);
        Task<TemplateViewModel> LoadTemplateAsync();
    }

}
