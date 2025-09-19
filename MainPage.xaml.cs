using IdentityCardGenerator.ViewModels;
#if WINDOWS
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
#endif


namespace IdentityCardGenerator;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

    //   public MainPage( MainViewModel mainViewModel)
    //{
    //	InitializeComponent();
    //	_viewModel = mainViewModel;
    //	BindingContext = _viewModel;
    //}

    public MainPage()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        BindingContext = _viewModel;
    }

    private async void OnBrowseExcelClicked(object sender, EventArgs e)
	{
		var result = await FilePicker.PickAsync(new PickOptions
		{
			FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
			{
				{ DevicePlatform.WinUI, new[] { ".xlsx", ".xls" } }
			}),
			PickerTitle = "Select Excel File"
		});

		if (result != null)
		{
			_viewModel.ExcelFilePath = result.FullPath;
			ExcelFilePathEntry.Text = result.FullPath;
		}
	}

	private async void OnBrowsePhotoFolderClicked(object sender, EventArgs e)
	{
        // Folder picking in MAUI can be platform-specific
        // For simplicity, we'll use a file picker and get the directory from the selected file
        var folderPath = await PickFolderAsync();
        if (!string.IsNullOrEmpty(folderPath))
        {
            await DisplayAlert("Folder Selected", folderPath, "OK");
        }

        if (folderPath != null)
		{
			
			_viewModel.PhotoFolderPath = folderPath;
			PhotoFolderPathEntry.Text = folderPath;
		}
	}

	private async void OnLoadDataClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(_viewModel.ExcelFilePath))
		{
			await DisplayAlert("Error", "Please select an Excel file first", "OK");
			return;
		}

		// Execute the command directly since it's a synchronous operation in our implementation
		_viewModel.LoadDataCommand.Execute(null);
	}

	private async void OnGenerateIdCardsClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(_viewModel.ExcelFilePath))
		{
			await DisplayAlert("Error", "Please select an Excel file first", "OK");
			return;
		}

		// Execute the command directly since it's a synchronous operation in our implementation
		_viewModel.GenerateIdCardsCommand.Execute(null);
	}
    public async Task<string?> PickFolderAsync()
    {
		#if WINDOWS
			var folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
			folderPicker.FileTypeFilter.Add("*"); // Required, even if you want folders only

			// Attach the picker to the current window
			var window = App.Current.Windows[0].Handler.PlatformView;
			var hwnd = WindowNative.GetWindowHandle(window);
			InitializeWithWindow.Initialize(folderPicker, hwnd);

			StorageFolder folder = await folderPicker.PickSingleFolderAsync();
			return folder?.Path;
		#else
				await DisplayAlert("Not supported", "Folder picking works only on Windows.", "OK");
				return null;
		#endif
    }
}
