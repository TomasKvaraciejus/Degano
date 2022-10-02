using CoreLocation;
using Degano.Controls;
using MapKit;
using Metal;
using Microsoft.Maui.Handlers;
using IMap = Degano.Controls.IMap;

namespace Degano.Handlers
{
    public partial class MapHandler : ViewHandler<IMap, MKMapView>
    {
        MKMapView? _mkMapView;
        CLLocationManager? _locationManager;

        protected override MKMapView CreatePlatformView()
        {
            _mkMapView = new MKMapView();
            return _mkMapView;
        }

        protected override void ConnectHandler(MKMapView platformView)
        {
            base.ConnectHandler(platformView);
            _locationManager = new CLLocationManager();
        }

        protected override void DisconnectHandler(MKMapView platformView)
        {
            _mkMapView?.Dispose();

            base.DisconnectHandler(platformView);
        }
        public static void MapIsShowingUser(IMapHandler handler, IMap map)
        {
#if !MACCATALYST
            if (map.IsShowingUser)
            {
                MapHandler? mapHandler = handler as MapHandler;
                mapHandler?._locationManager?.RequestWhenInUseAuthorization();
            }
#endif
            handler.PlatformView.ShowsUserLocation = map.IsShowingUser;
        }

        public static void MapIsScrollEnabled(IMapHandler handler, IMap map)
        {
            handler.PlatformView.ScrollEnabled = map.IsScrollEnabled;
        }

        public static void MapIsTrafficEnabled(IMapHandler handler, IMap map)
        {
            handler.PlatformView.ShowsTraffic = map.IsTrafficEnabled;
        }

        public static void MapIsZoomEnabled(IMapHandler handler, IMap map)
        {
            handler.PlatformView.ZoomEnabled = map.IsZoomEnabled;
        }

        public static void MapMinZoomLevel(IMapHandler handler, IMap map)
        {
            // Not currently implemented
        }

        public static void MapMaxZoomLevel(IMapHandler handler, IMap map)
        {
            // Not currently implemented
        }

        public static void MapSetBounds(IMapHandler handler, IMap map)
        {
            // Not currently implemented
        }

        public static void MapAddMarker(IMapHandler handler, IMap map, object? args)
        {
            // Not currently implemented
        }
    }
}
