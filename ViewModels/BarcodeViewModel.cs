using IdentityCardGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace IdentityCardGenerator.ViewModels
{
    public class BarcodeViewModel:INotifyPropertyChanged
    {
        private readonly IBarcodeService _barcodeService;
        private ImageSource _barcodeImage;

        public ImageSource BarcodeImage
        {
            get => _barcodeImage;
            set { _barcodeImage = value; OnPropertyChanged(nameof(BarcodeImage)); }
        }

        public BarcodeViewModel(IBarcodeService barcodeService)
        {
            _barcodeService = barcodeService;
            Generate();
        }

        private async void Generate()
        {
            BarcodeImage = await _barcodeService.GenerateBarcodeAsync(
                "ID-123456789", BarcodeFormat.CODE_128, 600, 200);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
