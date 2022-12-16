using Degano.SqliteDb;
using System.Collections.Generic;
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

        var discounts = GetCardDetails();
        if (discounts != null)
        {
            string type = ((KeyValuePair<String, bool>)GasStationSelect.SelectedItem).Key;

            Cards card = await Database.GetCardAsync(UserInfo.EMail, type);
            if (card == null)
            {
                card = new Cards { Email = UserInfo.EMail, CardName = type };
                UpdateCard(card, await discounts);
                await Database.InsertCardAsync(card);
            }
            else
            {
                UpdateCard(card, await discounts);
                await Database.UpdateCardAsync(card);
            }

            refresh.Invoke();
            await Navigation.PopAsync();
        }
    }

    private async Task<SortedDictionary<string, double>> GetCardDetails()
    {
        SortedDictionary<string, double> discounts = new SortedDictionary<string, double>()
        {
            { "95", -1 },
            { "98", -1 },
            { "Diesel", -1 },
            { "LPG", -1 }
        };

        string fuelType = await DisplayActionSheet("Select discounted fuel type(s): ", "Cancel", null, "All", "95", "98", "LPG", "Diesel");

        if (fuelType != null)
        {
            string discount = "";

            if (fuelType == "All")
            {
                discount = await DisplayPromptAsync("Discount", "Enter the discount in ct/l", "OK", "Cancel", "0", 2, Keyboard.Numeric);

                if (string.IsNullOrEmpty(discount))
                    return null;

                var _discounts = new SortedDictionary<string, double>();
                foreach (string type in discounts.Keys)
                {
                    _discounts.Add(type, int.Parse(discount));
                }
                return _discounts;
            }
            else
            {
                discount = await DisplayPromptAsync("Discount", "Enter the discount for " + fuelType + " in ct/l", "OK", "Cancel", "0", 2, Keyboard.Numeric);
                if (!string.IsNullOrEmpty(discount))
                {
                    discounts[fuelType] = int.Parse(discount);
                }
                else
                    return null;
            }

            return discounts;
        }
        else
            return null;
    }

    public static SortedDictionary<string, double> ReadCard(Cards card)
    {
        SortedDictionary<string, double> discounts = new SortedDictionary<string, double>();

        discounts.Add("95", card.Discount95 / 100.0);
        discounts.Add("98", card.Discount98 / 100.0);
        discounts.Add("LPG", card.DiscountLPG / 100.0);
        discounts.Add("Diesel", card.DiscountDiesel / 100.0);

        return discounts;
    }

    private static Cards UpdateCard(Cards card, SortedDictionary<string, double> discounts)
    {
        if (discounts["95"] != -1)  card.Discount95 = (int)discounts["95"];
        if (discounts["98"] != -1)  card.Discount98 = (int)discounts["98"];
        if (discounts["LPG"] != -1) card.DiscountLPG = (int)discounts["LPG"];
        if (discounts["Diesel"] != -1)  card.DiscountDiesel = (int)discounts["Diesel"];

        return card;
    }
}
