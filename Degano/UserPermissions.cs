using System;
using System.Collections.Generic;
using System.Text;
using Degano.Views;

namespace Degano
{
    public static class UserPermissions
    {
        private static PermissionStatus locationPermission;
        public static bool locationPermissionStatus { private set; get; }

        public static async Task GetPermissions()
        {
            try
            {
                locationPermission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if(locationPermission == PermissionStatus.Granted)
                    locationPermissionStatus = true;
            }
            catch
            {
                // Not currently implemented
            }
        }
    }
}
