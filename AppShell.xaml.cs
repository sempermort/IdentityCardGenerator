using Microsoft.Maui.Controls;
using IdentityCardGenerator.Views;

namespace IdentityCardGenerator;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for navigation
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(IdCardTemplate), typeof(IdCardTemplate));
		Routing.RegisterRoute(nameof(TemplateForm), typeof(TemplateForm));
		Routing.RegisterRoute(nameof(IdCardResults), typeof(IdCardResults));
    }
}
