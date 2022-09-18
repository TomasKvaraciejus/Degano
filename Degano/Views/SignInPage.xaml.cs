namespace Degano.Views;

public partial class SignInPage : ContentPage
{
	public string eMail { get; set; }
    public string Password { get; set; }
    public SignInPage()
	{
		InitializeComponent();
	}

	private void OnNameComplete(object sender, EventArgs e)
	{
		eMail=((Entry)sender).Text;
	}

	private void OnPasswordComplete(object sender, EventArgs e)
	{
        Password = ((Entry)sender).Text;
    }

	private void OnSubmitClick(object sender, EventArgs e)
	{
		Navigation.PopAsync();
    }
}