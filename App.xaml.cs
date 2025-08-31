using Microsoft.Maui.Controls;

namespace IdentityCardGenerator;

public partial class App : Application
{
	public App(IServiceProvider services)
	{
		InitializeComponent();

        MainPage = new AppShell(); // <-- Make sure you're setting Shell
                                   //// Resolve the MainPage from the service provider
                                   //var mainPage = services.GetService<MainPage>();
                                   //if (mainPage != null)
                                   //{
                                   //	MainPage = new NavigationPage(mainPage);
                                   //}
                                   //else
                                   //{
                                   //	// Fallback to default MainPage if not registered
                                   //	MainPage = new NavigationPage(new MainPage());
                                   //}
    }

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(MainPage);
	}
}