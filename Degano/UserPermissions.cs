using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Degano.Views;

namespace Degano
{
    [ExcludeFromCodeCoverage]
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
            catch(Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
            }
        }
    }
}
