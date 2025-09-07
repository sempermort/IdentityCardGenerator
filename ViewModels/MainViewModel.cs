using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IdentityCardGenerator.Models;
using IdentityCardGenerator.Services;

namespace IdentityCardGenerator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ExcelService _excelService;
        private readonly BarcodeService _barcodeService;
        private readonly PhotoService _photoService;
        
        private ObservableCollection<IdentityCard> _identityCards;
        private string _excelFilePath;
        private string _photoFolderPath;
        private string _statusMessage;
        private bool _isBusy;

        public MainViewModel()
        {
            _excelService = new ExcelService();
            _barcodeService = new BarcodeService();
            _photoService = new PhotoService();
            
            _identityCards = new ObservableCollection<IdentityCard>();
            
            LoadDataCommand = new Command(async () => await LoadDataAsync());
            GenerateIdCardsCommand = new Command(async () => await GenerateIdCardsAsync());
            NavigateToIdCardTemplateCommand = new Command(async () => await NavigateToIdCardTemplateAsync());
            NavigateToTemplateFormCommand = new Command(async () => await NavigateToTemplateFormAsync());
        }

        public ObservableCollection<IdentityCard> IdentityCards
        {
            get => _identityCards;
            set
            {
                _identityCards = value;
                OnPropertyChanged();
            }
        }

        public string ExcelFilePath
        {
            get => _excelFilePath;
            set
            {
                _excelFilePath = value;
                OnPropertyChanged();
            }
        }

        public string PhotoFolderPath
        {
            get => _photoFolderPath;
            set
            {
                _photoFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand GenerateIdCardsCommand { get; }
        public ICommand NavigateToIdCardTemplateCommand { get; }
        public ICommand NavigateToTemplateFormCommand { get; }

        private async Task LoadDataAsync()
        {
            if (string.IsNullOrEmpty(ExcelFilePath) || !File.Exists(ExcelFilePath))
            {
                StatusMessage = "Please select a valid Excel file";
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = "Loading data from Excel...";

                var cards = await _excelService.ReadIdentityCardsFromExcelAsync(ExcelFilePath);
                IdentityCards = new ObservableCollection<IdentityCard>(cards);

                StatusMessage = $"Loaded {cards.Count} records from Excel";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GenerateIdCardsAsync()
        {
            if (IdentityCards.Count == 0)
            {
                StatusMessage = "No data loaded. Please load data first.";
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = "Generating ID cards...";

                // Create directory for generated barcodes
                var barcodeDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GeneratedBarcodes");
                if (!Directory.Exists(barcodeDirectory))
                {
                    Directory.CreateDirectory(barcodeDirectory);
                }

                // Process each identity card
                for (int i = 0; i < IdentityCards.Count; i++)
                {
                    var card = IdentityCards[i];
                    StatusMessage = $"Processing card {i + 1} of {IdentityCards.Count}...";

                    // Generate barcode
                    var barcodeFileName = $"{card.IdNumber}_barcode.png";
                    var barcodePath = Path.Combine(barcodeDirectory, barcodeFileName);
                    
                    // Save barcode to file
                    var barcodeBytes = _barcodeService.GenerateBarcode(card.IdNumber);
                    await File.WriteAllBytesAsync(barcodePath, barcodeBytes);
                    card.BarcodePath = barcodePath;

                    // Find photo if photo folder path is provided
                    if (!string.IsNullOrEmpty(PhotoFolderPath) && Directory.Exists(PhotoFolderPath))
                    {
                        card.PhotoPath = _photoService.GetPhotoPathForId(card.IdNumber, PhotoFolderPath);
                    }

                    // In a real application, you would generate the actual ID card here
                    // This might involve creating a PDF or image file with the person's info,
                    // photo, and barcode
                }

                StatusMessage = "ID cards generated successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error generating ID cards: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task NavigateToIdCardTemplateAsync()
        {
            // Navigate to the ID card template page
            await Shell.Current.GoToAsync("//IdCardTemplate");
        }

        private async Task NavigateToTemplateFormAsync()
        {
            // Navigate to the template form page
            await Shell.Current.GoToAsync("//TemplateForm");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}