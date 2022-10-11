namespace Degano.Views
{
	public partial class MainPage : ContentPage
	{
		private static Controls.Map mainPageMap;
        private static List<GasStation> gasStationList = new List<GasStation>();

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
            await _p.Navigation.PushAsync(new MainPage());
        }

		public static async void InitializeMap()
		{
			mainPageMap.MapBounds = (54.765296, 25.371505, 54.619564, 25.146730); // These parameters are necessary for Google Maps to initialize properly
			mainPageMap.IsTrafficEnabled = false;
			mainPageMap.MinZoomLevel = 10f;
			mainPageMap.MaxZoomLevel = 16f;

            if (UserPermissions.locationPermissionStatus)
            {
                mainPageMap.IsShowingUser = true;
                await UserLocation.GetLastKnownLocation(); // preliminary
                mainPageMap.AnimateCamera((UserLocation.location, 14f, 800)); // preliminary
            }

            GetGasStationData();
		}

        public async static void GetGasStationData()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("data-test.txt");

            using (StreamReader file = new StreamReader(stream))
            {
                int counter = 0;
                string line, brand = "", name = "", address = "";
                double lat = 0.0, lng = 0.0;
                double dieselPrice = 0, petrol95Price = 0, petrol98Price = 0, LPGPrice = 0;
                while ((line = file.ReadLine()) != null)
                {
                    System.Diagnostics.Debug.WriteLine(line);
                    counter++;
                    switch (counter)
                    {
                        case 1:
                            brand = line;
                            break;
                        case 2:
                            name = line;
                            break;
                        case 3:
                            address = line;
                            break;
                        case 4:
                            lat = double.Parse(line);
                            break;
                        case 5:
                            lng = double.Parse(line);
                            break;
                        case 6:
                            if(line == "-")
                                dieselPrice = -1;
                            else
                                dieselPrice = double.Parse(line);
                            break;
                        case 7:
                            if (line == "-")
                                petrol95Price = -1;
                            else
                                petrol95Price = double.Parse(line);
                            break;
                        case 8:
                            if (line == "-")
                                petrol98Price = -1;
                            else
                                petrol98Price = double.Parse(line);
                            break;
                        case 9:
                            if (line == "-")
                                LPGPrice = -1;
                            else
                                LPGPrice = double.Parse(line);
                            var gasStation = new GasStation(name, address, new Location(lat, lng), petrol95Price, petrol98Price, dieselPrice, LPGPrice, brand);
                            mainPageMap.AddMarker(gasStation); // The rest of the function is used to create a marker for a single gas station
                                                               // on the map for debugging purposes
                            gasStationList.Add(gasStation);
                            counter = 0;
                            break;
                    }
                }
            }
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

        public void OnCenterUserClick(object sender, EventArgs e)
        {
            mainPageMap.AnimateCamera((UserLocation.location, 16f, 0));
        }

		private async void OnINeedGasClick(object sender, EventArgs e)
		{
			await DisplayAlert("STOP", "You need gas", "OK");
		}
	}
}
