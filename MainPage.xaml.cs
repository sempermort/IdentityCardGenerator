using IdentityCardGenerator.ViewModels;
using System.Windows.Input;

namespace IdentityCardGenerator;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

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
		var result = await FilePicker.PickAsync(new PickOptions
		{
			PickerTitle = "Select a file in the photo folder"
		});

		if (result != null)
		{
			var folderPath = Path.GetDirectoryName(result.FullPath);
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
}
