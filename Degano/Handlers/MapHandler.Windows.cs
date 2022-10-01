using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using IMap = Degano.Controls.IMap;

namespace Degano.Handlers
{
    public partial class MapHandler : ViewHandler<IMap, FrameworkElement>
    {
        protected override FrameworkElement CreatePlatformView() => throw new PlatformNotSupportedException();

        public static void MapHasZoomEnabled(IMapHandler handler, IMap map) => throw new PlatformNotSupportedException("No Map control on Windows.");

        public static void MapHasScrollEnabled(IMapHandler handler, IMap map) => throw new PlatformNotSupportedException("No Map control on Windows.");

        public static void MapHasTrafficEnabled(IMapHandler handler, IMap map) => throw new PlatformNotSupportedException("No Map control on Windows.");

        public static void MapIsShowingUser(IMapHandler handler, IMap map) => throw new PlatformNotSupportedException("No Map control on Windows.");

        public static void MapMinZoomLevel(IMapHandler handler, IMap map) => throw new PlatformNotSupportedException("No Map control on Windows.");

        public static void MapMaxZoomLevel(IMapHandler handler, IMap map) => throw new PlatformNotSupportedException("No Map control on Windows.");

        public static void MapSetBounds(IMapHandler handler, IMap map) => throw new PlatformNotSupportedException("No Map control on Windows.");

        public static void MapAddMarker(IMapHandler handler, IMap map, object? args) => throw new PlatformNotSupportedException("No Map control on Windows.");
    }
}
