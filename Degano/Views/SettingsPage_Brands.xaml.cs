using System.Diagnostics.CodeAnalysis;

namespace Degano.Views;

[ExcludeFromCodeCoverage]
public partial class SettingsPage_Brands : ContentPage
{
	public SettingsPage_Brands()
	{
		InitializeComponent();
        GasStationSelect.ItemsSource = GasStation.selectedGasStations;
    }

    private async void OnGasStationToggle(object sender, CheckedChangedEventArgs e)
    {
        var cb = (CheckBox)sender;
        var kvp = (KeyValuePair<string, bool>)cb.BindingContext;

        GasStation.selectedGasStations[kvp.Key] = e.Value;
    }
}
