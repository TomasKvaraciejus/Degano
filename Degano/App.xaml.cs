using Degano.Views;
namespace Degano;
public partial class App : Application
{
	public App(LandingPage landingPage, NavigationPage navigationPage)
	{
		InitializeComponent();
        navigationPage.PushAsync(landingPage);
		MainPage = navigationPage;
	}
}
