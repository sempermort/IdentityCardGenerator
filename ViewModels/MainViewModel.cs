using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IdentityCardGenerator.Models;
using IdentityCardGenerator.Services;
using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Views;

namespace IdentityCardGenerator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IExcelService _excelService;

        private readonly IIdCardDocument _documentService;
        private ObservableCollection<IdentityCard> _identityCards;
        private string? _excelFilePath;
        private string? _photoFolderPath;
        private string? _statusMessage;
        private bool _isBusy;
    public MainViewModel(IIdCardDocument documentService, IExcelService excelService)
    {
        _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        _identityCards = new ObservableCollection<IdentityCard>();

        LoadDataCommand = new Command(async () => await LoadDataAsync());
        GenerateIdCardsCommand = new Command(() => GenerateIdCards());
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

                var cards = await _excelService.ReadIdentityCardsFromExcelAsync(ExcelFilePath,PhotoFolderPath);
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



        private async void GenerateIdCards()
        {
            if (IdentityCards.Count == 0)
            {
                await NavigateToResultPage("No data loaded. Please load data first.", null);
                return;
            }

            string generatedFolderPath = null;

            try
            {
                IsBusy = true;
                StatusMessage = "Generating ID cards...";

                // Create directory for generated IDs
                var appDataDir = FileSystem.AppDataDirectory;
                generatedFolderPath = Path.Combine(appDataDir, "GeneratedIds");
                if (!Directory.Exists(generatedFolderPath))
                    Directory.CreateDirectory(generatedFolderPath);

                _documentService.SaveAllAsPdf(IdentityCards, generatedFolderPath);

                await NavigateToResultPage("ID cards generated successfully!", generatedFolderPath);
            }
            catch (Exception ex)
            {
                await NavigateToResultPage($"Error generating ID cards: {ex.Message}", null);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task NavigateToResultPage(string message, string folderPath)
        {
            string route = $"IdCardResults?statusMessage={Uri.EscapeDataString(message)}&folderPath={Uri.EscapeDataString(folderPath ?? "")}";
            await Shell.Current.GoToAsync(route);
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