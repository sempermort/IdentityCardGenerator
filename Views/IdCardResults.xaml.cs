namespace IdentityCardGenerator.Views;

[QueryProperty(nameof(StatusMessage), "statusMessage")]
[QueryProperty(nameof(FolderPath), "folderPath")]
public partial class IdCardResults : ContentPage
{
     
    private string _statusMessage;
    private string _folderPath;

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = Uri.UnescapeDataString(value ?? string.Empty);
            StatusLabel.Text = _statusMessage;
        }
    }

    public string FolderPath
    {
        get => _folderPath;
        set
        {
            _folderPath = Uri.UnescapeDataString(value ?? string.Empty);
            OpenFolderButton.IsVisible = Directory.Exists(_folderPath);
        }
    }

    public IdCardResults()
    {
        InitializeComponent();
    }

    private async void OpenFolderButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (Directory.Exists(_folderPath))
            {
                await Launcher.OpenAsync(_folderPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Cannot open folder: {ex.Message}", "OK");
        }
    }
}