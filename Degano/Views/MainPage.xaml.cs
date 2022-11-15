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

        // We need to implement a system to only load gas stations if they are within a certain range of the user / 
        // in the viewable area of their screen
        public async static void GetGasStationData()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                //AuthSecret = "mpbd3Up8Hykggejr0R8IRtsUhnnLrfwjNtPnmVaJ",
                BasePath = "https://degano-70426-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            try
            {
                IFirebaseClient client = new FirebaseClient(config);
                FirebaseResponse response = await client.GetAsync("Degano/");
                Dictionary<string, DatabaseEntry> data = JsonConvert.DeserializeObject<Dictionary<string, DatabaseEntry>>(response.Body.ToString());
                foreach(var item in data)
                {
                    double dieselPrice = double.Parse(item.Value.diesel);
                    double lpgPrice;
                    if (item.Value.lpg != "-")
                    {
                        lpgPrice = double.Parse(item.Value.lpg);
                    }
                    else
                    {
                        lpgPrice = 0;
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
                        petrol98Price = 0;
                    }
                    GasStation gasStation = new GasStation(item.Value.name, item.Value.address, new Location(lat, lng), 
                        petrol95Price, petrol98Price, dieselPrice, lpgPrice, item.Value.brand);
                    gasStation.GetDistanceToUser();
                    mainPageMap.AddMarker(gasStation);
                    gasStationList.Add(gasStation);
                }

                GasStation.preferredPriceMin = gasStationList.Min(g => g.price95);
                GasStation.preferredPriceMax = gasStationList.Max(g => g.price95);
            }
            catch
            {
                // Error handling here
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
            await UserLocation.GetLastKnownLocation();
            mainPageMap.AnimateCamera((UserLocation.location, 16f, 0));
        }

        private async Task<GasStation> FindGasStation()
        {
            if (gasStationList.Count == 0)
                throw new Exception("gasStationList empty");
            // we need to keep track of user's distance to all GasStations and update it regularly, so this function should also be invoked in other functions
            gasStationList.ForEach(g => g.GetDistanceToUser());
            // finds GasStation with highest appealCoef within specified distance
            var g = gasStationList.Where(g => g.distance < GasStation.distMax).Aggregate((g1, g2) => g1.appealCoef < g2.appealCoef ? g2 : g1); 
            return g;
        }

        private async void OnINeedGasClick(object sender, EventArgs e)
		{
            try
            {
                await UserLocation.GetLastKnownLocation();
                GasStation g = await FindGasStation();
                mainPageMap.AnimateCamera((g.location, 16f, 0));
                mainPageMap.SelectGasStation(g);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
            }
		}
	}
}
