using Android.OS;
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
        SettingsPage settingsPage;
        public MainPage(SettingsPage _settingsPage)
        {
            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, false);
            mainPageMap = MainPageMap;
            UserLocation.LocationAvailableChanged += ToggleINeedGas;
            settingsPage = _settingsPage;
            ToggleINeedGas();
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
                catch (Exception ex)
                {
                    ExceptionLogger.Log(ex.Message);
                }
            }

            await GetGasStationData();
        }

        public static void AddMarkersToMap<T>(List<T> items)
        {
            foreach (T item in items)
                mainPageMap.AddMarker(item);
        }

        public static double ToDouble<T>(T arg)
        {
            return (double)Convert.ChangeType(arg, typeof(double));
        }

        // We need to implement a system to only load gas stations if they are within a certain range of the user / 
        // in the viewable area of their screen
        public async static Task GetGasStationData()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                BasePath = "https://degano-70426-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            try
            {
                GasStation.gasStationList.ForEach(g => mainPageMap.RemoveGasStation(g));
                mainPageMap.Clear();
                while (GasStation.gasStationList.Count > 0)
                {
                    GasStation.gasStationList.RemoveAt(0);
                }
                IFirebaseClient client = new FirebaseClient(config);
                FirebaseResponse response = await client.GetAsync("Degano/");
                Dictionary<string, DatabaseEntry> data = JsonConvert.DeserializeObject<Dictionary<string, DatabaseEntry>>(response.Body.ToString());
                Func<string, double> parser = ToDouble;
                foreach(var item in data)
                {
                    Lazy<GasStation> g;
                    double dieselPrice = parser(item.Value.diesel);
                    double lpgPrice;
                    if (item.Value.lpg != "-")
                    {
                        lpgPrice = parser(item.Value.lpg);
                    }
                    else
                    {
                        lpgPrice = -1;
                    }
                    double lat = parser(item.Value.lat);
                    double lng = parser(item.Value.lng);
                    double petrol95Price = parser(item.Value.petrol95);
                    double petrol98Price;
                    if (item.Value.petrol98 != "-")
                    {
                        petrol98Price = parser(item.Value.petrol98);
                    }
                    else
                    {
                        petrol98Price = -1;
                    }
                    g = new Lazy<GasStation>(() => new GasStation(item.Value.name, item.Value.address, new Location(lat, lng), 
                        petrol95Price, petrol98Price, dieselPrice, lpgPrice, item.Value.brand));

                    g.Value.GetDistanceToUser();
                    GasStation.gasStationList.Add(g.Value);
                }

                AddMarkersToMap(GasStation.gasStationList);
                GasStation.preferredPriceMin = GasStation.gasStationList.Min(g => g.price95);
                GasStation.preferredPriceMax = GasStation.gasStationList.Max(g => g.price95);
            }
            catch (Exception ex)
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
            if (UserInfo.EMail != null)
            {
                settingsPage.Title = UserInfo.EMail + " " +settingsPage.Title;
                Navigation.PushAsync(settingsPage);
            }
            else
            {
                DisplayAlert("Error", "Please sign in or sign up first!", "OK");
            }
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
