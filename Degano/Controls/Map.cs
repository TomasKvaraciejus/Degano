using System.Diagnostics.CodeAnalysis;

namespace Degano.Controls
{
    [ExcludeFromCodeCoverage]
    public partial class Map : View, IMap
    {

        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create(nameof(IsShowingUser), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty IsTrafficEnabledProperty = BindableProperty.Create(nameof(IsTrafficEnabled), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty IsScrollEnabledProperty = BindableProperty.Create(nameof(IsScrollEnabled), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty IsZoomEnabledProperty = BindableProperty.Create(nameof(IsZoomEnabled), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty MinZoomLevelProperty = BindableProperty.Create(nameof(MinZoomLevel), typeof(float), typeof(Map), null);

        public static readonly BindableProperty MaxZoomLevelProperty = BindableProperty.Create(nameof(MaxZoomLevel), typeof(float), typeof(Map), null);

        public static readonly BindableProperty MapBoundsProperty = BindableProperty.Create(nameof(MapBounds), typeof((Location, Location)), typeof(Map), null);

        public bool IsScrollEnabled
        {
            get { return (bool)GetValue(IsScrollEnabledProperty); }
            set { SetValue(IsScrollEnabledProperty, value); }
        }

        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        public bool IsShowingUser
        {
            get { return (bool)GetValue(IsShowingUserProperty); }
            set { SetValue(IsShowingUserProperty, value); }
        }

        public bool IsTrafficEnabled
        {
            get => (bool)GetValue(IsTrafficEnabledProperty);
            set => SetValue(IsTrafficEnabledProperty, value);
        }

        public float MinZoomLevel
        {
            get { return (float)GetValue(MinZoomLevelProperty); }
            set { SetValue(MinZoomLevelProperty, value); }
        }

        public float MaxZoomLevel
        {
            get { return (float)GetValue(MaxZoomLevelProperty); }
            set { SetValue(MaxZoomLevelProperty, value); }
        }

        public (Location, Location) MapBounds
        {
            get { return ((Location, Location))GetValue(MapBoundsProperty); }
            set { SetValue(MapBoundsProperty, value); }
        }

        public void AddMarker(object? args)
        {
            Handler?.Invoke(nameof(AddMarker), args);
        }

        public void AnimateCamera(object? args)
        {
            Handler?.Invoke(nameof(AnimateCamera), args);
        }

        public void MoveCamera(object? args)
        {
            Handler?.Invoke(nameof(MoveCamera), args);
        }

        public void ZoomCamera(object? args)
        {
            Handler?.Invoke(nameof(ZoomCamera), args);
        }

        public void SelectGasStation(object? args)
        {
            Handler?.Invoke(nameof(SelectGasStation), args);
        }

        public void RemoveGasStation(object? args)
        {
            Handler?.Invoke(nameof(RemoveGasStation), args);
        }

        public void Clear()
        {
            Handler?.Invoke(nameof(Clear), null);
        }
    }
}
