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
                    CompanyName = template.CompanyName,
                    HeaderText = template.HeaderText,
                    FooterText = template.FooterText,
                    BackgroundColor = template.BackgroundColor,
                    TextColor = template.TextColor,
                    BorderColor = template.BorderColor,
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
                    return new TemplateViewModel();
                }

                var json = await File.ReadAllTextAsync(_templateFilePath);
                var templateData = JsonSerializer.Deserialize<TemplateData>(json);

                if (templateData != null)
                {
                    var template = new TemplateViewModel
                    {
                        CompanyName = templateData.CompanyName,
                        HeaderText = templateData.HeaderText,
                        FooterText = templateData.FooterText,
                        BackgroundColor = templateData.BackgroundColor,
                        TextColor = templateData.TextColor,
                        BorderColor = templateData.BorderColor,
                        ShowBarcode = templateData.ShowBarcode,
                        ShowPhoto = templateData.ShowPhoto,
                        ShowSignatureLine = templateData.ShowSignatureLine
                    };
                    return template;
                }

                // Return default template if deserialization fails
                return new TemplateViewModel();
            }
            catch (Exception ex)
            {
                // Log exception or handle as appropriate for your application
                throw new Exception($"Error loading template: {ex.Message}", ex);
            }
        }
    }


}