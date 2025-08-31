using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IdentityCardGenerator.Views
{
    public partial class IdCardTemplate : ContentPage, INotifyPropertyChanged
    {
        private string _companyName;
        private string _borderColor;
        private string _textColor;
        private bool _showBarcode;
        private bool _showPhoto;
        private bool _showSignatureLine;

        public IdCardTemplate()
        {
            InitializeComponent();
            InitializeDefaultValues();
            BindingContext = this;
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

        public string BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
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

        private void InitializeDefaultValues()
        {
            CompanyName = "COMPANY NAME";
            BorderColor = "#000000";
            TextColor = "#000000";
            ShowBarcode = true;
            ShowPhoto = true;
            ShowSignatureLine = true;
        }

        private async void OnSaveIdCardClicked(object sender, EventArgs e)
        {
            // In a real application, this would save the ID card as an image or PDF
            await DisplayAlert("Success", "ID Card saved successfully!", "OK");
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}