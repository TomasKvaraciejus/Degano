using System.Net.Http.Json;

namespace Degano
{
    public class GasStation : 
#if ANDROID
        Java.Lang.Object, // Necessary for casting GasStation to java object in MapHandler.Android.cs
#endif
    IComparable
    {
        public static List<GasStation> gasStationList = new List<GasStation>();
        public static List<GasStation> enabledGasStationList = new List<GasStation>();
        public static SortedDictionary<string, bool> selectedGasStations = new SortedDictionary<string, bool>();
        public static string selectedType = "95";

        public string name, address, type; // type variable denotes gas station company (e.g. "Viada", "Circle-K"), whereas name can store entire name of gas station (i.e. "Viada pilaite")
        public double appealCoef { get { return GetAppeal(); } }
        public Location location;
        public SortedDictionary<string, double> fuelPrice = new SortedDictionary<string, double>()
        {
            { "95", -1 },
            { "98", -1 },
            { "Diesel", -1 },
            { "LPG", -1 }
        };
        public double distance;
        private const int R = 6371; // radius of the earth
        private const double _r = 0.01745329251; // used for converting coordinates to rad
        public static double preferredPriceMin = -1;
        public static double preferredPriceMax = -1;
        public static double distMax = 2; // maximum distance to search for gas stations (probably user-defined)
        private const double wDist = 0.4;
        private const double wPrice = 0.6;
        private static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://maps.googleapis.com/maps/api/distancematrix/json"),
        };
        
        public GasStation(string _name, string _address, Location _location, double _price95, double _price98, double _priceDiesel, double _priceLPG, string _brand)
        {
            name = _name;
            address = _address;
            location = _location;
            distance = -1;
            fuelPrice["95"] = _price95;
            fuelPrice["98"] = _price98;
            fuelPrice["Diesel"] = _priceDiesel;
            fuelPrice["LPG"] = _priceLPG;
            type = _brand;
        }

        public GasStation() { }

        public static async Task UpdateAllDistances()
        {
            List<GasStation> currentGasStations = new List<GasStation>();
            foreach(GasStation g in enabledGasStationList)
            {
                currentGasStations.Add(g);
                if (currentGasStations.Count == 25) // The maximum number of destinations in a Google Matrix API request is 25
                {
                    await GetDrivingDistanceToUser(currentGasStations);
                }
            }
            await GetDrivingDistanceToUser(currentGasStations);
        }

        private static async Task GetDrivingDistanceToUser(List<GasStation> currentGasStations)
        {
            string coords = String.Join("|", currentGasStations.Select(g => g.location.lat.ToString() + ',' + g.location.lng.ToString()).ToArray());

            string request = $"?origins={UserLocation.location.lat},{UserLocation.location.lng}" +
                     $"&destinations={coords}&sensor=false" +
                     $"&key=AIzaSyBbkz9JBShE8JYmYFoU2XG-jqIigrR4jyg";
            HttpResponseMessage response = await httpClient.GetAsync(request);
            GoogleMapResponse r = await response.Content.ReadFromJsonAsync<GoogleMapResponse>();

            int i = 0;
            foreach (GasStation _g in currentGasStations)
            {
                _g.distance = r.Rows[0].Elements[i].Distance.Value / 1000.0;
                ++i;
            }

            if(currentGasStations.Count != 0)
                currentGasStations.Clear();
        }

        public async Task GetDrivingDistanceToUser()
        {
            string request = $"?origins={UserLocation.location.lat},{UserLocation.location.lng}" +
                             $"&destinations={location.lat},{location.lng}&sensor=false" +
                             $"&key=AIzaSyBbkz9JBShE8JYmYFoU2XG-jqIigrR4jyg";
            HttpResponseMessage response = await httpClient.GetAsync(request);
            GoogleMapResponse r = await response.Content.ReadFromJsonAsync<GoogleMapResponse>();

            distance = r.Rows[0].Elements[0].Distance.Value/1000.0;
        }

        public void GetDistanceToUser() // Should probably be moved to Location
        {
            // We could get the distance using the Google Maps API as well, this would result in more accurate results
            // (it would account for the actual distance required to drive), unfortunately this would result in an excess
            // of calls and probably wouldn't be super efficient. 
            // Apparently doing this over Google's API would cost 5$/1000calls, and you can calculate up to 25 distances over
            // one call. Still way more expensive than literally free, but this method gives us skewed data as it doesn't
            // factor in driving distance. It'll work for now. 

            if (location.lat == -1 || location.lng == -1)
                throw new Exception("GasStation location invalid");
            if (UserLocation.location.lat == -1 || UserLocation.location.lng == -1)
                throw new Exception("User location invalid");

            double gLat = location.lat * _r;
            double uLat = UserLocation.location.lat * _r;
            double deltaLat = (UserLocation.location.lat - location.lat) * _r;
            double deltaLng = (UserLocation.location.lng - location.lng) * _r;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(gLat) * Math.Cos(uLat) *
                       Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

            distance = c * R;
        }

        public static async Task<GasStation> FindGasStation()
        {
            if (enabledGasStationList.Count == 0)
                throw new Exception("gasStationList empty");
            // we need to keep track of user's distance to all GasStations and update it regularly, so this function should also be invoked in other functions
            //enabledGasStationList.ForEach(g => g.GetDistanceToUser());

            //enabledGasStationList.ForEach(g => g.GetDrivingDistanceToUser());
            // finds GasStation with highest appealCoef within specified distance
            var g = enabledGasStationList.Where(g => g.distance < distMax).ToList();
            if (g.Count == 0)
                throw new Exception("no GasStations under max range");

            preferredPriceMax = g.Max(g1 => g1.fuelPrice[selectedType]);
            preferredPriceMin = g.Min(g2 => g2.fuelPrice[selectedType]);

            return g.Aggregate((g1, g2) => g1.appealCoef < g2.appealCoef ? g2 : g1);
        }

        private double GetAppeal()
        {
            if (preferredPriceMax == preferredPriceMin)
            {
                if (preferredPriceMax == -1)
                    throw new Exception("GasStation price range undefined");
                else
                    preferredPriceMin = 0;
            }
            if(distance == -1)
            {
                throw new Exception("GasStation distance undefined");
            }

            double price = ((preferredPriceMax - fuelPrice[selectedType]) / (preferredPriceMax - preferredPriceMin)) + 0.5;
            double dist = (distMax - distance) / distMax;
            return ((price * price * wPrice) + (dist * wDist));
        }

        int IComparable.CompareTo(object? obj) // This function is only preliminary
        {
            var _gasStation = (GasStation)obj;

            if (_gasStation == null)
                return 1;
            else
                return this.distance.CompareTo(_gasStation.distance);
        }

        public static int CompareAppeal(GasStation g1, GasStation g2)
        {
            return g2.appealCoef.CompareTo(g1.appealCoef);
        }
    }
}
