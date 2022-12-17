using System.Reflection;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Degano.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Debug = System.Diagnostics.Debug;
using IMap = Degano.Controls.IMap;

namespace Degano.Handlers
{
    public partial class MapHandler : ViewHandler<IMap, MapView>
    {
        private static Dictionary<GasStation, Marker> gasStationMap = new Dictionary<GasStation, Marker>();

        MapView? _mapView;
        MapCallbackHandler? _mapReady;

        private static Dictionary<string, Bitmap> GasStationResources = new Dictionary<string, Bitmap>();
        private static Bitmap GasStation_Default;

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
                    googleMap.MyLocationEnabled = true;
                else
                {
                    Debug.WriteLine("Missing location permissions for IsShowingUser.");
                    googleMap.MyLocationEnabled = false;
                }
            }
            else
            {
                googleMap.MyLocationEnabled = false;
            }

            googleMap.UiSettings.MyLocationButtonEnabled = false;
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

            var sw = new LatLng(map.MapBounds.Item2.lat, map.MapBounds.Item2.lng);
            var ne = new LatLng(map.MapBounds.Item1.lat, map.MapBounds.Item1.lng);
            googleMap.SetLatLngBoundsForCameraTarget(new LatLngBounds(sw, ne));
        }

        public static async void MapAddMarker(IMapHandler handler, IMap map, object? args)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            var g = (GasStation)args;

            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(g.location.lat, g.location.lng));
            marker.SetTitle(g.name);

            if (GasStationResources.ContainsKey(g.type.ToLower()))
            {
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(GasStationResources[g.type.ToLower()]));
            }
            else
            {
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(GasStation_Default));
            }

            var _marker = googleMap.AddMarker(marker); // AddMarker returns new marker
            gasStationMap.Add(g, _marker);
            _marker.Tag = g; // Tags GasStation object to marker for use in displaying custom info-window
        }

        public static void MapAnimateCamera(IMapHandler handler, IMap map, object? args)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            var _args = ((Location, float, int))args; // args should consist of Location, Zoom (0 if zoom is to remain unchanged), Animation Length (>0 for custom animation length)
            var _latlng = new LatLng(_args.Item1.lat, _args.Item1.lng);
            var _zoom = googleMap.CameraPosition.Zoom;
            var _animationLength = 800;
            if (_args.Item2 != 0)
                _zoom = _args.Item2;
            if (_args.Item3 > 0)
                _animationLength = _args.Item3;
            var cameraPosition = CameraUpdateFactory.NewCameraPosition(new CameraPosition(_latlng, _zoom, 0, 0));

            googleMap.AnimateCamera(cameraPosition, _animationLength, null);
        }

        public static void MapMoveCamera(IMapHandler handler, IMap map, object? args)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            var _args = ((Location, float, int))args; 
            var _latlng = new LatLng(_args.Item1.lat, _args.Item1.lng);

            googleMap.AnimateCamera(CameraUpdateFactory.NewLatLng(_latlng), 800, null);
        }

        public static void MapZoomCamera(IMapHandler handler, IMap map, object? args)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            var _args = (float)args;

            googleMap.AnimateCamera(CameraUpdateFactory.ZoomTo(_args), 800, null);
        }

        public static void MapSelectGasStation(IMapHandler handler, IMap map, object? args)
        {
            GoogleMap? googleMap = handler?.Map;
            if (googleMap == null)
                return;

            gasStationMap[(GasStation)args].ShowInfoWindow();
        }

        public static void MapRemoveGasStation(IMapHandler handler, IMap map, object? args)
        {
            GoogleMap? googleMap = handler?.Map;
            GasStation g = (GasStation)args;
            if (googleMap == null || !gasStationMap.ContainsKey(g))
                return;

            Marker marker = gasStationMap[g];
            marker.Remove();

            gasStationMap.Remove(g);
        }

        private async void LoadResources()
        {
            try
            {
                var imgSrc = ImageSource.FromResource("Degano.Resources.Images.gasstationdefault.png");
                var _bitmap = await new ImageLoaderSourceHandler().LoadImageAsync(imgSrc, Android.App.Application.Context, CancellationToken.None);
                GasStation_Default = Bitmap.CreateScaledBitmap(_bitmap, 130, 130, true);

                var assembly = Assembly.GetExecutingAssembly();
                foreach (var name in assembly.GetManifestResourceNames())
                {
                    if (name.StartsWith("Degano.Resources.Images"))
                    {
                        var gasStationName = name.Substring("Degano.Resources.Images.gasstation".Length);

                        imgSrc = ImageSource.FromResource(name);
                        _bitmap = await new ImageLoaderSourceHandler().LoadImageAsync(imgSrc, Android.App.Application.Context, CancellationToken.None);
                        GasStationResources.Add(gasStationName.Remove(gasStationName.IndexOf('.')), Bitmap.CreateScaledBitmap(_bitmap, 130, 130, true));
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionLogger.Log(ex.Message);
            }
        }

        internal void OnMapReady(GoogleMap map)
        {
            if (map == null)
                return;

            Map = map;
            var _infoWindowAdapter = new InfoWindowAdapter(this);
            Map.SetInfoWindowAdapter(_infoWindowAdapter);
            Map.SetOnInfoWindowClickListener(_infoWindowAdapter);
            var R = Android.App.Application.Context;
            Map.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(R, Resource.Raw.map_style));

            if(GasStationResources.Count == 0)
            {
                LoadResources();
            }
            Views.MainPage.InitializeMap();
        }
    }

    class InfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter, GoogleMap.IOnInfoWindowClickListener
    {
        private MapHandler mapHandler;

        public InfoWindowAdapter(MapHandler _mapHandler)
        {
            mapHandler = _mapHandler;
        }

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
            var address = (gasStation.address).Split(',');
            viewText.Text = address[0] + '\n' + address[1];

            viewText = (TextView)view.FindViewById(Resource.Id.station_price95);
            viewText.Text = "95:";
            if (gasStation.fuelPriceBase.value["95"] != -1)
                viewText.Text += gasStation.fuelPrice.value["95"].ToString("0.000").PadLeft(22 - viewText.Text.Length);
            else
                viewText.Text += "-".PadLeft(26 - viewText.Text.Length);

            viewText = (TextView)view.FindViewById(Resource.Id.station_price98);
            viewText.Text = "98:";
            if (gasStation.fuelPriceBase.value["98"] != -1)
                viewText.Text += gasStation.fuelPrice.value["98"].ToString("0.000").PadLeft(22 - viewText.Text.Length);
            else
                viewText.Text += "-".PadLeft(26 - viewText.Text.Length);

            viewText = (TextView)view.FindViewById(Resource.Id.station_priceDiesel);
            viewText.Text = "Diesel:";
            if (gasStation.fuelPriceBase.value["Diesel"] != -1)
                viewText.Text += gasStation.fuelPrice.value["Diesel"].ToString("0.000").PadLeft(20 - viewText.Text.Length);
            else
                viewText.Text += "-".PadLeft(24 - viewText.Text.Length);

            viewText = (TextView)view.FindViewById(Resource.Id.station_priceLPG);
            viewText.Text = "LPG:";
            if (gasStation.fuelPriceBase.value["LPG"] != -1)
                viewText.Text += gasStation.fuelPrice.value["LPG"].ToString("0.000").PadLeft(20 - viewText.Text.Length);
            else
                viewText.Text += "-".PadLeft(24 - viewText.Text.Length);

            viewText = (TextView)view.FindViewById(Resource.Id.station_distance);
            if (gasStation.distance != -1)
            {
                string dist;
                if (gasStation.distance > 1)
                    dist = Math.Round((gasStation.distance), 1).ToString() + "km";
                else
                    dist = Math.Round(gasStation.distance * 1000, 0).ToString() + "m";
                viewText.Text = "Distance: " + dist;
            }
            else
                viewText.Text += "err: distance not available";

            return view;
        }

        public void OnInfoWindowClick(Marker marker)
        {
            var gasStation = (GasStation)marker.Tag;
            gasStation.location.OpenInExternalApp();
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
