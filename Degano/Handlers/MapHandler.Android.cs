using Android;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Speech;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Java.Interop;
using Microsoft.Maui.Handlers;
using Debug = System.Diagnostics.Debug;
using IMap = Degano.Controls.IMap;

namespace Degano.Handlers
{
    public partial class MapHandler : ViewHandler<IMap, MapView>
    {
        MapView? _mapView;
        MapCallbackHandler? _mapReady;

        public GoogleMap? Map { get; set; }

        protected override MapView CreatePlatformView()
        {
            _mapView = new MapView(Context);
            _mapView.OnCreate(null);
            _mapView.OnResume();
            return _mapView;
        }

        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);

            _mapReady = new MapCallbackHandler(this);
            platformView.GetMapAsync(_mapReady);
        }

        protected override void DisconnectHandler(MapView platformView)
        {
            _mapReady = null;
            _mapView?.Dispose();

            base.DisconnectHandler(platformView);
        }

        public static void MapIsShowingUser(IMapHandler handler, IMap map)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            if (handler?.MauiContext?.Context == null)
                return;

            if (map.IsShowingUser)
            {
                var coarseLocationPermission = ContextCompat.CheckSelfPermission(handler.MauiContext.Context, Manifest.Permission.AccessCoarseLocation);
                var fineLocationPermission = ContextCompat.CheckSelfPermission(handler.MauiContext.Context, Manifest.Permission.AccessFineLocation);

                if (coarseLocationPermission == Permission.Granted || fineLocationPermission == Permission.Granted)
                    googleMap.MyLocationEnabled = googleMap.UiSettings.MyLocationButtonEnabled = true;
                else
                {
                    Debug.WriteLine("Missing location permissions for IsShowingUser.");
                    googleMap.MyLocationEnabled = googleMap.UiSettings.MyLocationButtonEnabled = false;
                }
            }
            else
            {
                googleMap.MyLocationEnabled = googleMap.UiSettings.MyLocationButtonEnabled = false;
            }
        }

        public static void MapIsScrollEnabled(IMapHandler handler, IMap map)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            googleMap.UiSettings.ScrollGesturesEnabled = map.IsScrollEnabled;
        }

        public static void MapIsTrafficEnabled(IMapHandler handler, IMap map)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            googleMap.TrafficEnabled = map.IsTrafficEnabled;
        }

        public static void MapIsZoomEnabled(IMapHandler handler, IMap map)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            googleMap.UiSettings.ZoomControlsEnabled = map.IsZoomEnabled;
            googleMap.UiSettings.ZoomGesturesEnabled = map.IsZoomEnabled;
        }

        public static void MapMinZoomLevel (IMapHandler handler, IMap map)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            googleMap.SetMinZoomPreference(map.MinZoomLevel);
        }

        public static void MapMaxZoomLevel(IMapHandler handler, IMap map)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            googleMap.SetMaxZoomPreference(map.MaxZoomLevel);
        }

        public static void MapSetBounds(IMapHandler handler, IMap map)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            var sw = new LatLng(map.MapBounds.Item3, map.MapBounds.Item4);
            var ne = new LatLng(map.MapBounds.Item1, map.MapBounds.Item2);
            googleMap.SetLatLngBoundsForCameraTarget(new LatLngBounds(sw, ne));
        }

        public static void MapAddMarker(IMapHandler handler, IMap map, object? args)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            var _args = (GasStation)args;

            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(_args.location.Item1, _args.location.Item2));
            marker.SetTitle(_args.name);

            var _marker = googleMap.AddMarker(marker); // AddMarker returns new marker
            _marker.Tag = _args; // Tags GasStation object to marker for use in displaying custom info-window
        }
        
        internal void OnMapReady(GoogleMap map)
        {
            if (map == null)
                return;

            Map = map;
            Map.SetInfoWindowAdapter(new InfoWindowAdapter());
            var R = Android.App.Application.Context;
            Map.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(R, Resource.Raw.map_style));

            Views.MainPage.InitializeMap();
        }
    }

    class InfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            var gasStation = marker.Tag as GasStation;
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            var view = inflater.Inflate(Resource.Layout.info_window, null);

            var viewText = (TextView)view.FindViewById(Resource.Id.station_name);
            viewText.Text = gasStation.name;
            viewText = (TextView)view.FindViewById(Resource.Id.station_address);
            viewText.Text = gasStation.address;
            viewText = (TextView)view.FindViewById(Resource.Id.station_price95);
            viewText.Text += gasStation.price95;

            return view;
        }
    }

    class MapCallbackHandler : Java.Lang.Object, IOnMapReadyCallback
    {
        MapHandler _handler;
        GoogleMap? _googleMap;

        public MapCallbackHandler(MapHandler mapHandler)
        {
            _handler = mapHandler;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;
            _handler.OnMapReady(googleMap);
        }
    }
}
