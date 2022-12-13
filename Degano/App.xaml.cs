using Degano.Views;
using System.Diagnostics.CodeAnalysis;

namespace Degano;
[ExcludeFromCodeCoverage]
public partial class App : Application
{
    public App(LandingPage landingPage, NavigationPage navigationPage)
	{
		InitializeComponent();
        navigationPage.PushAsync(landingPage);
		MainPage = navigationPage;
	}
}
