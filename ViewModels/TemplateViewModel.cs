using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IdentityCardGenerator.Services;

namespace IdentityCardGenerator.ViewModels
{
    public class TemplateViewModel : INotifyPropertyChanged
    {
        private readonly TemplateService _templateService;
        private string _companyName;
        private string _headerText;
        private string _footerText;
        private string _backgroundColor;
        private string _textColor;
        private string _borderColor;
        private bool _showBarcode;
        private bool _showPhoto;
        private bool _showSignatureLine;

        public TemplateViewModel()
        {
            _templateService = new TemplateService();
            
            _companyName = "COMPANY NAME";
            _headerText = "EMPLOYEE ID CARD";
            _footerText = "If found, please return to the address above";
            _backgroundColor = "#FFFFFF";
            _textColor = "#000000";
            _borderColor = "#000000";
            _showBarcode = true;
            _showPhoto = true;
            _showSignatureLine = true;

            SaveTemplateCommand = new Command(async () => await SaveTemplateAsync());
            ResetToDefaultCommand = new Command(ResetToDefault);
            LoadTemplateCommand = new Command(async () => await LoadTemplateAsync());
            
            // Load saved template when view model is created
            _ = LoadTemplateAsync();
        }

        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged();
            }
        }

        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                OnPropertyChanged();
            }
        }

        public string FooterText
        {
            get => _footerText;
            set
            {
                _footerText = value;
                OnPropertyChanged();
            }
        }

        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public string TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                OnPropertyChanged();
            }
        }

        public string BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                OnPropertyChanged();
            }
        }

        public bool ShowBarcode
        {
            get => _showBarcode;
            set
            {
                _showBarcode = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPhoto
        {
            get => _showPhoto;
            set
            {
                _showPhoto = value;
                OnPropertyChanged();
            }
        }

        public bool ShowSignatureLine
        {
            get => _showSignatureLine;
            set
            {
                _showSignatureLine = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveTemplateCommand { get; }
        public ICommand ResetToDefaultCommand { get; }
        public ICommand LoadTemplateCommand { get; }

        private async Task SaveTemplateAsync()
        {
            try
            {
                await _templateService.SaveTemplateAsync(this);
                await Application.Current.MainPage.DisplayAlert("Success", "Template saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error saving template: {ex.Message}", "OK");
            }
        }

        private async Task LoadTemplateAsync()
        {
            try
            {
                var savedTemplate = await _templateService.LoadTemplateAsync();
                
                // Update properties with saved values
                CompanyName = savedTemplate.CompanyName;
                HeaderText = savedTemplate.HeaderText;
                FooterText = savedTemplate.FooterText;
                BackgroundColor = savedTemplate.BackgroundColor;
                TextColor = savedTemplate.TextColor;
                BorderColor = savedTemplate.BorderColor;
                ShowBarcode = savedTemplate.ShowBarcode;
                ShowPhoto = savedTemplate.ShowPhoto;
                ShowSignatureLine = savedTemplate.ShowSignatureLine;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error loading template: {ex.Message}", "OK");
            }
        }

        private void ResetToDefault()
        {
            CompanyName = "COMPANY NAME";
            HeaderText = "EMPLOYEE ID CARD";
            FooterText = "If found, please return to the address above";
            BackgroundColor = "#FFFFFF";
            TextColor = "#000000";
            BorderColor = "#000000";
            ShowBarcode = true;
            ShowPhoto = true;
            ShowSignatureLine = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}