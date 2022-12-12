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
			UserName.IsVisible = false;
			SignInButton.IsVisible = true;
			SignUpButton.IsVisible = true;
		}
		else
		{
			UserName.IsVisible = true;
			UserName.Text = UserInfo.EMail;
            SignInButton.IsVisible = false;
            SignUpButton.IsVisible = false;
        }
	}

	private async void OnSignIn(object sender, EventArgs e)
    {

	}

	private async void OnSignUp(object sender, EventArgs e)
    {

	}
}
