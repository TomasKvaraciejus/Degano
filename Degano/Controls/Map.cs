using Android.OS;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace Degano.Controls
{
    public partial class Map : View, IMap
    {

        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create(nameof(IsShowingUser), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty HasTrafficEnabledProperty = BindableProperty.Create(nameof(HasTrafficEnabled), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create(nameof(HasScrollEnabled), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create(nameof(HasZoomEnabled), typeof(bool), typeof(Map), null);

        public static readonly BindableProperty MinZoomLevelProperty = BindableProperty.Create(nameof(MinZoomLevel), typeof(float), typeof(Map), null);

        public static readonly BindableProperty MaxZoomLevelProperty = BindableProperty.Create(nameof(MaxZoomLevel), typeof(float), typeof(Map), null);

        public static readonly BindableProperty MapBoundsProperty = BindableProperty.Create(nameof(MapBounds), typeof((double, double, double, double)), typeof(Map), null);

        public bool HasScrollEnabled
        {
            get { return (bool)GetValue(HasScrollEnabledProperty); }
            set { SetValue(HasScrollEnabledProperty, value); }
        }

        public bool HasZoomEnabled
        {
            get { return (bool)GetValue(HasZoomEnabledProperty); }
            set { SetValue(HasZoomEnabledProperty, value); }
        }

        public bool IsShowingUser
        {
            get { return (bool)GetValue(IsShowingUserProperty); }
            set { SetValue(IsShowingUserProperty, value); }
        }

        public bool HasTrafficEnabled
        {
            get => (bool)GetValue(HasTrafficEnabledProperty);
            set => SetValue(HasTrafficEnabledProperty, value);
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

        public (double, double, double, double) MapBounds
        {
            get { return ((double, double, double, double))GetValue(MapBoundsProperty); }
            set { SetValue(MapBoundsProperty, value); }
        }

        public void AddMarker(object? args)
        {
            Handler?.Invoke(nameof(AddMarker), args);
        }
    }
}
