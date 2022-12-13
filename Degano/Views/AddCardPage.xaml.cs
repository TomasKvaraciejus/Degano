using Degano.SqliteDb;
using System.Diagnostics.CodeAnalysis;

namespace Degano.Views;

[ExcludeFromCodeCoverage]
public partial class AddCardPage : ContentPage
{

    SqliteDatabase Database;
    public Action refresh;
    public AddCardPage(SqliteDatabase databe)
	{
		InitializeComponent();
        GasStationSelect.ItemsSource = GasStation.selectedGasStations;
        Database = databe;
    }

    private async void AddCard(object? sender, EventArgs e)
    {
        if (GasStationSelect.SelectedItem == null)
        {
            return;
        }
        var discount = await DisplayPromptAsync("Discount", "Enter the discount in ct/l", "OK", "Cancel", "0", 2, Keyboard.Numeric);
        if (!string.IsNullOrEmpty(discount))
        {
            KeyValuePair<String, bool> a = (KeyValuePair<String, bool>)GasStationSelect.SelectedItem;
            await Database.InsertCardAsync(new Cards { Email = UserInfo.EMail, CardName = a.Key, Discount = int.Parse(discount) });
            refresh.Invoke();
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Alert", "Please enter a discount", "OK");
        }
    }
}
