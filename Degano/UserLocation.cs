using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Degano
{
    public static class UserLocation
    {
        public static Location location;
        public static bool isLocationAvailable;
        private static CancellationTokenSource _cancelTokenSource;
        private static bool _isCheckingLocation;
        public delegate void PropertyChanged();
        public static event PropertyChanged LocationAvailableChanged;

        static UserLocation()
        {
            isLocationAvailable = false;
            location.lng = -1;
            location.lat = -1;
        }
        public static async Task GetLastKnownLocation()
        {
            try
            {
                var _location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (_location != null)
                {
                    location.lng = _location.Longitude;
                    location.lat = _location.Latitude;
                }
                else
                {
                    throw new Exception("Unable to get last-known user location");
                }

                isLocationAvailable = true;
                LocationAvailableChanged();
            }
            catch(Exception ex)
            {
                isLocationAvailable = false;
                LocationAvailableChanged();
                ExceptionLogger.Log(ex.Message);
            }
        }
        public static async Task GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                _cancelTokenSource = new CancellationTokenSource();
                _isCheckingLocation = true;
                var _location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (_location != null)
                {
                    location.lng = _location.Longitude;
                    location.lat = _location.Latitude;
                }
                else
                {
                    throw new Exception("Unable to get user location");
                }

                isLocationAvailable = true;
                LocationAvailableChanged();
                _isCheckingLocation = false;
            }
            catch(Exception ex)
            {
                isLocationAvailable = false;
                LocationAvailableChanged();
                ExceptionLogger.Log(ex.Message);
            }
        }

        public static void CancelLocationRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
                _cancelTokenSource.Cancel();
        }
    }
}
