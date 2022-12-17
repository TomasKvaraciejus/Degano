using Degano.SqliteDb;
using Microsoft.Maui.Controls;
using System.Diagnostics.CodeAnalysis;

namespace Degano.Views;

[ExcludeFromCodeCoverage]
public partial class CardPage : ContentPage
{
    SqliteDatabase Database;
    AddCardPage AddCardpage;

    private SortedDictionary<string, TextCell> textCells = new SortedDictionary<string, TextCell>();

    public CardPage(SqliteDatabase database, AddCardPage addCardPage)
    {
        InitializeComponent();
        addCardPage.refresh = LoadCards;
        Database = database;
        AddCardpage = addCardPage;
    }

    public async void LoadCards()
    {
        var cards = await Database.GetCardsAsync(UserInfo.EMail);
        if (cards.Count == 0) return;
        int[] classIds = new int[CardsItem.Count];
        for (int i=0; i<CardsItem.Count; i++)
        {
            classIds[i] = int.Parse(CardsItem.ElementAt(i).ClassId);
        }
        foreach (Cards card in cards)
        {
            var discounts = AddCardPage.ReadCard(card);

            if (!textCells.ContainsKey(card.CardName))
            {
                bool legit = true;
                for (int i = 0; i < classIds.Length; i++)
                {
                    if (classIds[i] == card.id)
                    {
                        legit = false;
                    }
                }
                if (legit)
                {
                    var textCell = new TextCell { Text = card.CardName, ClassId = card.id.ToString() };
                    string details = "";
                    foreach(string type in discounts.Keys)
                    {
                        if (discounts[type] != -0.01)
                            details += discounts[type] * 100 + " ct/l (" + type + ") | ";
                    }
                    textCell.Detail = details.Substring(0, details.Length - 2);
                    textCell.Tapped += async (object? sender, EventArgs e) =>
                    {
                        bool delete = await DisplayAlert("Alert", "Do you want to delete this card?", "YES", "NO");
                        if (delete)
                        {
                            await Database.DeleteCardAsync(UserInfo.EMail, card.CardName);
                            GasStation.discounts.Remove(card.CardName);
                            textCells.Remove(card.CardName);
                            CardsItem.Remove(textCell);
                        }
                    };
                    GasStation.discounts.Add(card.CardName, new FuelType(discounts));
                    textCells.Add(card.CardName, textCell);
                    CardsItem.Add(textCell);
                    LoadCards();
                }
            }
            else
            {
                GasStation.discounts[card.CardName] = new FuelType(discounts);
                string details = "";
                foreach (string type in discounts.Keys)
                {
                    details += discounts[type] * 100 + " ct/l (" + type + ") | ";
                }
                textCells[card.CardName].Detail = details.Substring(0, details.Length - 2);
            }
        }
    }

    public async void OnAddCard(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(AddCardpage);
        LoadCards();
    }

    public async void OnRefresh(object? sender, EventArgs e)
    {
        LoadCards();
    }
}

