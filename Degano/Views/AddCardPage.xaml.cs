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
            string type = ((KeyValuePair<String, bool>)GasStationSelect.SelectedItem).Key;

            Cards card = await Database.GetCardAsync(UserInfo.EMail, type);
            if (card == null)
            {
                await Database.InsertCardAsync(new Cards { Email = UserInfo.EMail, CardName = type, Discount = int.Parse(discount) });
            }
            else
            {
                card.Discount = int.Parse(discount);
                await Database.UpdateCardAsync(card);
            }

            refresh.Invoke();
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Alert", "Please enter a discount", "OK");
        }
    }
}
