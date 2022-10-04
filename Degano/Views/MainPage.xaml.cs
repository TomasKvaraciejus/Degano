namespace Degano.Views
{
	public partial class MainPage : ContentPage
	{
		private static Controls.Map mainPageMap;

		// We should probably figure out a way to keep the map in between
		// content pages, otherwise a new one to be generated every time this                                                                              
		// page is opened

		public MainPage()
		{
			InitializeComponent();
			mainPageMap = MainPageMap;
		}

		internal static async void InitializeMainPage(ContentPage _p)
		{
            await UserPermissions.GetPermissions();
            _p.Navigation.PushAsync(new MainPage());
        }

		public static void InitializeMap()
		{
			mainPageMap.MapBounds = (54.765296, 25.371505, 54.619564, 25.146730); // These parameters are necessary for Google Maps to initialize properly
			mainPageMap.IsTrafficEnabled = false;
			mainPageMap.MinZoomLevel = 10f;
			mainPageMap.MaxZoomLevel = 16f;

			if (UserPermissions.locationPermissionStatus)
				mainPageMap.IsShowingUser = true;

			var gasStation = new GasStation("Viada", "Pilaite", (54.7, 25.2), 0, 0, 0, 0, "Viada"); // The rest of the function is used to create a marker for a single gas station
			mainPageMap.AddMarker(gasStation);                                                      // on the map for debugging purposes
		}

		// The map has to be initialized every time this window is opened
		// due to:
		// A - the android handler not setting the properties of the map
		// screen (**probably** because it calls an async method to generate
		// the map and continues attempting to set the properties in tandem
		// but is unable to because the map does not exist yet)
		// B - the android handler refusing the set the properties to the
		// defaults provided in Degano.Controls.Map.cs even after successful
		// generation of the map resulting in them having to be set to null

		public void OnSettingsClick(object sender, EventArgs e)
		{

		}

		private async void OnINeedGasClick(object sender, EventArgs e)
		{
			await DisplayAlert("STOP", "You need gas", "OK");
		}
	}
}
