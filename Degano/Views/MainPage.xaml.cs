using System.Diagnostics;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

namespace Degano.Views
{
	public partial class MainPage : ContentPage
	{
		private static Controls.Map mainPageMap;
        public delegate void INeedGasToggleHandler();

        // We should probably figure out a way to keep the map in between
        // content pages, otherwise a new one to be generated every time this                                                                              
        // page is opened

        public MainPage()
		{
			InitializeComponent();
			mainPageMap = MainPageMap;
            UserLocation.LocationAvailableChanged += ToggleINeedGas;
            ToggleINeedGas();
		}

		internal static async void InitializeMainPage(ContentPage _p)
		{
            await UserPermissions.GetPermissions();
            await _p.Navigation.PushAsync(new MainPage());
        }

		public static async void InitializeMap()
		{
			mainPageMap.MapBounds = (new Location(54.765296, 25.371505), new Location(54.619564, 25.146730)); // These parameters are necessary for Google Maps to initialize properly
			mainPageMap.IsTrafficEnabled = false;
			mainPageMap.MinZoomLevel = 10f;
			mainPageMap.MaxZoomLevel = 16f;

            if (UserPermissions.locationPermissionStatus)
            {
                try
                {
                    mainPageMap.IsShowingUser = true;
                    await UserLocation.GetLastKnownLocation(); // preliminary
                    mainPageMap.AnimateCamera((UserLocation.location, 14f, 800)); // preliminary
                }
                catch(Exception ex)
                {
                    ExceptionLogger.Log(ex.Message);
                }
            }

            GetGasStationData();
		}

        // We need to implement a system to only load gas stations if they are within a certain range of the user / 
        // in the viewable area of their screen
        public async static void GetGasStationData()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                BasePath = "https://degano-70426-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            try
            {
                IFirebaseClient client = new FirebaseClient(config);
                FirebaseResponse response = await client.GetAsync("Degano/");
                Dictionary<string, DatabaseEntry> data = JsonConvert.DeserializeObject<Dictionary<string, DatabaseEntry>>(response.Body.ToString());
                foreach(var item in data)
                {
                    Lazy<GasStation> gasStation;
                    double dieselPrice = double.Parse(item.Value.diesel);
                    double lpgPrice;
                    if (item.Value.lpg != "-")
                    {
                        lpgPrice = double.Parse(item.Value.lpg);
                    }
                    else
                    {
                        lpgPrice = -1;
                    }
                    double lat = double.Parse(item.Value.lat);
                    double lng = double.Parse(item.Value.lng);
                    double petrol95Price = double.Parse(item.Value.petrol95);
                    double petrol98Price;
                    if (item.Value.petrol98 != "-")
                    {
                        petrol98Price = double.Parse(item.Value.petrol98);
                    }
                    else
                    {
                        petrol98Price = -1;
                    }
                    gasStation = new Lazy<GasStation>(() => new GasStation(item.Value.name, item.Value.address, new Location(lat, lng), 
                        petrol95Price, petrol98Price, dieselPrice, lpgPrice, item.Value.brand));
                    gasStation.Value.GetDistanceToUser();
                    mainPageMap.AddMarker(gasStation.Value);
                    GasStation.gasStationList.Add(gasStation.Value);
                }

                GasStation.preferredPriceMin = GasStation.gasStationList.Min(g => g.price95);
                GasStation.preferredPriceMax = GasStation.gasStationList.Max(g => g.price95);
            }
            catch(Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
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

        public async void OnCenterUserClick(object sender, EventArgs e)
        {
            try
            {
                await UserLocation.GetLastKnownLocation();
                mainPageMap.AnimateCamera((UserLocation.location, 16f, 0));
            }
            catch(Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
            }
        }

        private async void OnINeedGasClick(object sender, EventArgs e)
		{
            try
            {
                await UserLocation.GetLastKnownLocation();
                GasStation g = await GasStation.FindGasStation();
                mainPageMap.AnimateCamera((g.location, 16f, 0));
                mainPageMap.SelectGasStation(g);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
            }
		}

        private void ToggleINeedGas()
        {
            if (UserLocation.isLocationAvailable)
            {
                GasButton.BackgroundColor = Colors.Red;
                GasButton.IsEnabled = true;
            }
            else
            {
                GasButton.BackgroundColor = Colors.Grey;
                GasButton.IsEnabled = false;
            }
        }
	}
}
