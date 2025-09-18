using Microsoft.Extensions.Logging;
using IdentityCardGenerator.Services;
using IdentityCardGenerator.ViewModels;
using IdentityCardGenerator.Views;
using IdentityCardGenerator.Interfaces;
using IdentityCardGenerator.Helpers;


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
			});

		// Register services
		builder.Services.AddSingleton<IExcelService, ExcelService>();
		builder.Services.AddSingleton<IBarcodeService, BarcodeService>();
		builder.Services.AddSingleton<IPhotoService, PhotoService>();
		builder.Services.AddSingleton<ITemplateService, TemplateService>();
        builder.Services.AddSingleton<IIdCardDocument, IdCardDocument>();


        // Register view models
        builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<TemplateViewModel>();
		
		// Register views
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<IdCardTemplate>();
		builder.Services.AddTransient<TemplateForm>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
