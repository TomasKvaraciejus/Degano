#if IOS || MACCATALYST
using PlatformView = MapKit.MKMapView;
#elif ANDROID
using Android.Gms.Maps;
using PlatformView = Android.Gms.Maps.MapView;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.FrameworkElement;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0 && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif
using Microsoft.Maui.Handlers;
using IMap = Degano.Controls.IMap;

namespace Degano.Handlers;

public partial class MapHandler : IMapHandler
{
    IMap IMapHandler.VirtualView => VirtualView;

    PlatformView IMapHandler.PlatformView => PlatformView;

    public MapHandler() : base(Mapper, CommandMapper)
    {
    }

    public static IPropertyMapper<IMap, IMapHandler> Mapper = new PropertyMapper<IMap, IMapHandler>(ViewMapper)
    {
        [nameof(IMap.IsShowingUser)] = MapIsShowingUser,
        [nameof(IMap.IsScrollEnabled)] = MapIsScrollEnabled,
        [nameof(IMap.IsTrafficEnabled)] = MapIsTrafficEnabled,
        [nameof(IMap.IsZoomEnabled)] = MapIsZoomEnabled,
        [nameof(IMap.MinZoomLevel)] = MapMinZoomLevel,
        [nameof(IMap.MaxZoomLevel)] = MapMaxZoomLevel,
        [nameof(IMap.MapBounds)] = MapSetBounds
    };

    public static CommandMapper<IMap, IMapHandler> CommandMapper = new(ViewCommandMapper)
    {
        [nameof(IMap.AddMarker)] = MapAddMarker
    };
}
