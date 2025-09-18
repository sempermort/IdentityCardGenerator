using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IdentityCardGenerator.Helpers;
using IdentityCardGenerator.Models;
using IdentityCardGenerator.Services;
using ZXing;

namespace IdentityCardGenerator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ExcelService _excelService;

        private readonly IdCardDocument _documentService;
        private ObservableCollection<IdentityCard> _identityCards;
        private string? _excelFilePath;
        private string? _photoFolderPath;
        private string? _statusMessage;
        private bool _isBusy;

        public MainViewModel(IdCardDocument documentService, ExcelService excelService)
        {
            _excelService = excelService;
            _identityCards = new ObservableCollection<IdentityCard>();
            _documentService = documentService;
            LoadDataCommand = new Command(async () => await LoadDataAsync());
            GenerateIdCardsCommand = new Command(() => GenerateIdCards());
            NavigateToIdCardTemplateCommand = new Command(async () => await NavigateToIdCardTemplateAsync());
            NavigateToTemplateFormCommand = new Command(async () => await NavigateToTemplateFormAsync());
        }

        public MainViewModel()
        {
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
            get => _excelFilePath!;
            set
            {
                _excelFilePath = value;
                OnPropertyChanged();
            }
        }

        public string PhotoFolderPath
        {
            get => _photoFolderPath!;
            set
            {
                _photoFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage!;
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



        private void GenerateIdCards()
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
                var IdCardDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GeneratedIds");
                if (!Directory.Exists(IdCardDirectory))
                {
                    Directory.CreateDirectory(IdCardDirectory);
                    _documentService.SaveAllAsPdf(IdentityCards, IdCardDirectory);
                }

                // Process each identity card



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
        public void SaveAllAsPdf(ObservableCollection<IdentityCard> records)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "IdentityCards.pdf");

            if (IdentityCards != null && IdentityCards.Any())
            {
                _documentService.SaveAllAsPdf(records,path);
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}