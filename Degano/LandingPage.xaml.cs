namespace Degano;
using Degano.Views;
public partial class LandingPage : ContentPage
{
	public LandingPage()
	{
		InitializeComponent();
    }

    private void OnSignInClick(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SignInPage());
    }

    private void OnSignUpClick(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SignUpPage());
    }
    private void OnContinueWithoutSignInClick(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}