using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Services;
using IdentityCardGenerator.ViewModels;
using IdentityCardGenerator.Views;
using Microsoft.Extensions.Logging;


namespace IdentityCardGenerator;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
		
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                
                fonts.AddFont("Roboto-VariableFont_wdth,wght", "RobotoVariable");

                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
                fonts.AddFont("Poppins-Regular.ttf", "Poppins");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
            });

		// Register services
		builder.Services.AddSingleton<IExcelService, ExcelService>();
		builder.Services.AddSingleton<IBarcodeService, BarcodeService>();
		builder.Services.AddSingleton<IPhotoService, PhotoService>();
		builder.Services.AddSingleton<ITemplateService, TemplateService>();
        builder.Services.AddSingleton<IIdCardDocument, IdCardDocument>();


        // Register view models
        builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<TemplateViewModel>();
		
		// Register views
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<IdCardTemplate>();
		builder.Services.AddTransient<TemplateForm>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
