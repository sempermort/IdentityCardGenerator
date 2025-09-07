using System.Text.Json;
using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Models;
using IdentityCardGenerator.ViewModels;

namespace IdentityCardGenerator.Services
{
    public class TemplateService: ITemplateService
    {
        private readonly string _templateFilePath;

        public TemplateService()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _templateFilePath = Path.Combine(folderPath, "IdCardTemplateSettings.json");
        }

        public async Task SaveTemplateAsync(TemplateViewModel template)
        {
            try
            {
                var templateData = new TemplateData
                {
                    FirstName = template.FirstName,
                    LastName = template.LastName,
                    IdNumber = template.IdNumber, 
                    Department = template.Department,
                    Phone = template.Phone,
                    PhotoPath = template.PhotoPath,
                    BarcodePath = template.BarcodePath,
                    ShowBarcode = template.ShowBarcode,
                    ShowPhoto = template.ShowPhoto,
                    ShowSignatureLine = template.ShowSignatureLine
                };

                var json = JsonSerializer.Serialize(templateData, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_templateFilePath, json);
            }
            catch (Exception ex)
            {
                // Log exception or handle as appropriate for your application
                throw new Exception($"Error saving template: {ex.Message}", ex);
            }
        }

        public async Task<TemplateViewModel> LoadTemplateAsync()
        {
            try
            {
                if (!File.Exists(_templateFilePath))
                {
                    // Return default template if no saved template exists
                    // Fix: Set required member 'PhotoPath' in the object initializer for TemplateViewModel
                    return new TemplateViewModel { PhotoPath = string.Empty };
                }

                var json = await File.ReadAllTextAsync(_templateFilePath);
                var templateData = JsonSerializer.Deserialize<TemplateData>(json);

                if (templateData != null)
                {
                    var template = new TemplateViewModel
                    {
                        FirstName = templateData.FirstName,
                        LastName = templateData.LastName,
                        IdNumber = templateData.IdNumber, // You may want to set this from your ViewModel if available
                        Department = templateData.Department,
                        Phone = templateData.Phone,
                        PhotoPath = templateData.PhotoPath,
                        BarcodePath = templateData.BarcodePath,
                        ShowBarcode = templateData.ShowBarcode,
                        ShowPhoto = templateData.ShowPhoto,
                        ShowSignatureLine = templateData.ShowSignatureLine
                    };
                    return template;
                }

                // Return default template if deserialization fails
                // Fix: Set required member 'PhotoPath' in the object initializer for TemplateViewModel
                return new TemplateViewModel { PhotoPath = string.Empty };
            }
            catch (Exception ex)
            {
                // Log exception or handle as appropriate for your application
                throw new Exception($"Error loading template: {ex.Message}", ex);
            }
        }
    }


}