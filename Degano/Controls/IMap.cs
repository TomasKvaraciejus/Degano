namespace Degano.Controls
{
    public interface IMap : IView
    {
        bool IsShowingUser { get; }
        bool IsScrollEnabled { get; }
        bool IsZoomEnabled { get; }
        bool IsTrafficEnabled { get; }
        float MinZoomLevel { get; }
        float MaxZoomLevel { get; }
        (double, double, double, double) MapBounds { get; }

        // Adds Marker to map,
        // currently takes GasStation as argument to be tagged to marker
        void AddMarker(object? args);
    }
}
