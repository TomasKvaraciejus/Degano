namespace Degano.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnINeedGasClick(object sender, EventArgs e)
	{
        await DisplayAlert("STOP", "You need gas", "OK");
    }
}