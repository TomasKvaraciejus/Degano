using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Degano
{
    public static class UserLocation
    {
        public static Location location;
        private static CancellationTokenSource _cancelTokenSource;
        private static bool _isCheckingLocation;

        public static async Task GetLastKnownLocation()
        {
            var _location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (_location != null)
            {
                location.lng = _location.Longitude;
                location.lat = _location.Latitude;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unable to get last-known user location");
            }
        }
        public static async Task GetLocation()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            _cancelTokenSource = new CancellationTokenSource();
            _isCheckingLocation = true;
            var _location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if(_location != null)
            {
                location.lng = _location.Longitude;
                location.lat = _location.Latitude;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unable to get current user location");
            }

            _isCheckingLocation = false;
        }

        public static void CancelLocationRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
                _cancelTokenSource.Cancel();
        }
    }
}
