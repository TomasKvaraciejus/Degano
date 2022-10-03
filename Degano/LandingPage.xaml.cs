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

	private async void OnContinueWithoutSignInClick(object sender, EventArgs e)
	{
		await CheckPermissions();
		Navigation.PushAsync(new MainPage());
	}

    private async Task CheckPermissions() // We need a better way to keep track of user permissions
    {
        try
        {
            MainPage.locationPermission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        catch
        {
            // Not currently implemented
        }
        if (MainPage.locationPermission == PermissionStatus.Denied)
            await DisplayAlert("Location permissions denied",
                                "Your location permissions must be adjusted for the functionality of the \"I need gas!\" feature.", "okey");
    }
}
