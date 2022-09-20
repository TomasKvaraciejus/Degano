namespace Degano.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

        var mapView = new Mapsui.UI.Maui.MapView();
        mapView.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        mapView.IsClippedToBounds = true;
        MapView.Map = mapView.Map;
        mapView.MyLocationLayer.UpdateMyLocation(new Mapsui.UI.Maui.Position(54.676693963316616, 25.286771387708566)); // Issue: location is not updated but separate location icon
                                                                                                                       // is created if this is done here,
                                                                                                                       // resulting in two location icons on screen. 
                                                                                                                       // If done at any later point, location is updated according to
                                                                                                                       // parameters but initial location icon remains on screen. 
    }

    public void OnSettingsClick(object sender, EventArgs e)
    {

	}

	private async void OnINeedGasClick(object sender, EventArgs e)
	{
        await DisplayAlert("STOP", "You need gas", "OK");
    }
}