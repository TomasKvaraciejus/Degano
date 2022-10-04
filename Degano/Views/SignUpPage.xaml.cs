namespace Degano.Views;

public partial class SignUpPage : ContentPage
{
    public string EMail { get; set; }
    public string Password { get; set; }
    public SignUpPage()
    {
        InitializeComponent();
    }

    private void OnNameComplete(object sender, EventArgs e)
    {
        EMail = ((Entry)sender).Text;
    }

    private void OnPasswordComplete(object sender, EventArgs e)
    {
        Password = ((Entry)sender).Text;
    }

    private async void OnSubmitClick(object sender, EventArgs e)
    {
        await UserPermissions.GetPermissions();
        Navigation.PushAsync(new MainPage());
    }
}
