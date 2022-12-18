using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Degano.Views
{
    [ExcludeFromCodeCoverage]
    public partial class MainPage : ContentPage
    {
        private static Controls.Map mainPageMap;
        private static MainPage mainPage;
        public delegate void INeedGasToggleHandler();
        SettingsPage settingsPage;

        public MainPage(SettingsPage _settingsPage)
        {
            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, false);
            mainPageMap = MainPageMap;
            mainPage = this;
            UserLocation.LocationAvailableChanged += ToggleINeedGas;
            settingsPage = _settingsPage;
            settingsPage.UpdateTraffic = UpdateTraffic;
        }

        protected override bool OnBackButtonPressed() { return true; }

        public static async void InitializeMap()
        {
            mainPageMap.MapBounds = (new Location(0, 0), new Location(0, 0)); // bugfix
            mainPageMap.IsTrafficEnabled = true;
            mainPageMap.MinZoomLevel = 0f;
            mainPageMap.MaxZoomLevel = 0f;

            mainPageMap.MapBounds = (new Location(54.765296, 25.371505), new Location(54.619564, 25.146730)); // These parameters are necessary for Google Maps to initialize properly
            mainPageMap.IsTrafficEnabled = false;
            mainPageMap.MinZoomLevel = 10f;
            mainPageMap.MaxZoomLevel = 16f;

            if (UserPermissions.locationPermissionStatus)
            {
                try
                {
                    mainPageMap.IsShowingUser = false; // bugfix
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

        private static void UpdateTraffic(bool b)
        {
            mainPageMap.IsTrafficEnabled = b;
        }

        public static async Task AddMarkersToMap()
        {
            foreach (GasStation g in GasStation.enabledGasStationList)
            {
                mainPageMap.AddMarker(g);
            }
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
                GasStation.gasStationList.Clear();
                mainPageMap.Clear();
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
                    //g.Value.GetDrivingDistanceToUser();
                    GasStation.gasStationList.Add(g.Value);
                }

                GasStation.gasStationList.GroupBy(g => g.type).Select(g => g.First()).ToList().ForEach(g => GasStation.selectedGasStations.TryAdd(g.type, true));

                await UpdateShownGasStations();

                response = await client.GetAsync("Updated/Datetime");
                string updated = response.Body.ToString();
                updated = updated.Replace("\"", "");
                var updatedTimes = updated.Split(" ");

                DateTime updatedDate = new DateTime(int.Parse(updatedTimes[0]),
                                                    int.Parse(updatedTimes[1]),
                                                    int.Parse(updatedTimes[2]),
                                                    int.Parse(updatedTimes[3]),
                                                    int.Parse(updatedTimes[4]),
                                                    int.Parse(updatedTimes[5]));

                GasStation.lastUpdated = (DateTime.Now - updatedDate).Hours;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
            }
        }

        public static async Task UpdateShownGasStations()
        {

            foreach(GasStation g in GasStation.enabledGasStationList)
            {
                mainPageMap.RemoveGasStation(g);
            }
            GasStation.enabledGasStationList.Clear();
            foreach(GasStation g in GasStation.gasStationList)
            {
                if (GasStation.selectedGasStations[g.type] && g.fuelPriceBase.value[GasStation.selectedType] != -1)
                {
                    g.UpdatePrices();
                    GasStation.enabledGasStationList.Add(g);
                }
            }
            await AddMarkersToMap();

            ToggleINeedGas(GasStation.enabledGasStationList.Any(g => g.distance <= GasStation.distMax));
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

        public async void OnSettingsClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(settingsPage);
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
                await GasStation.UpdateAllDistances();
                await UserLocation.GetLastKnownLocation();
                /*var brand = GasStation.gasStationList.GroupBy(g => g.type).Select(g => g.First()).ToList();
                brand.ForEach(g => System.Diagnostics.Debug.WriteLine("aaa" + g.type));*/
                GasStation g = await GasStation.FindGasStation();
                mainPageMap.AnimateCamera((g.location, 16f, 0));

                mainPageMap.SelectGasStation(g);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
            }
		}

        private static void ToggleINeedGas(bool status)
        {
            if (status)
            {
                mainPage.GasButton.BackgroundColor = Color.FromRgba("FF4500");
                mainPage.GasButton.IsEnabled = true;
            }
            else
            {
                mainPage.GasButton.BackgroundColor = Colors.Grey;
                mainPage.GasButton.IsEnabled = false;
            }
        }
    }
}
