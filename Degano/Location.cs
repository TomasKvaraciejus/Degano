using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Degano
{
    public struct Location
    {
        public double lat { set; get; }
        public double lng { set; get; }

        public Location(double _lat, double _lng)
        {
            lat = _lat;
            lng = _lng;
        }

        public async void OpenInExternalApp() // Issue: Opens app in split-screen view
        {
            await Launcher.OpenAsync($"geo:?q={lat},{lng}"); // Opens specified app (Issue: Compatibility with platforms other than Android unknown)
            //await Map.OpenAsync(lat, lng); // Allows user to choose app to open (Issue: Opens native app, meaning that iOS may be forced to use Apple Maps)
        }
    }
}
