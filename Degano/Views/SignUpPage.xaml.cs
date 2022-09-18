namespace Degano.Views;

public partial class SignUpPage : ContentPage
{
    public string eMail { get; set; }
    public string Password { get; set; }
    public SignUpPage()
    {
        InitializeComponent();
    }

    private void OnNameComplete(object sender, EventArgs e)
    {
        eMail = ((Entry)sender).Text;
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