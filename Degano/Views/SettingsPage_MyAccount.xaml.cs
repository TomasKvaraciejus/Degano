namespace Degano.Views;

public partial class SettingsPage_MyAccount : ContentPage
{
	public SettingsPage_MyAccount()
	{
		InitializeComponent();
	}

	public void OnPageEntry()
	{
		if(UserInfo.EMail == null)
		{
			UserName.Text = "not signed in";
			SignInButton.IsVisible = true;
            SignOutButton.IsVisible = false;
        }
		else
		{
			UserName.Text = UserInfo.EMail;
            SignInButton.IsVisible = false;
			SignOutButton.IsVisible = true;
        }
	}

	private async void OnSignIn(object sender, EventArgs e)
    {
		await Navigation.PopToRootAsync();
	}

	private async void OnSignUp(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

	private async void OnSignOutButton(object sender, EventArgs e)
	{
		UserInfo.Password = UserInfo.EMail = null;
        await Navigation.PopToRootAsync();
    }
}
