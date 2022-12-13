using Degano.SqliteDb;
using Microsoft.Maui.Controls;

namespace Degano.Views;

public partial class CardPage : ContentPage
{
    SqliteDatabase Database;
    AddCardPage AddCardpage;
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
            bool legit = true;
            for (int i=0; i<classIds.Length; i++)
            {
                if (classIds[i] == card.id)
                {
                    legit = false;
                }
            }
            if (legit)
            {
                var textCell = new TextCell { Text = card.CardName, Detail = card.Discount.ToString() + " ct/l", ClassId = card.id.ToString() };
                textCell.Tapped += async (object? sender, EventArgs e) =>
                {
                    bool delete = await DisplayAlert("Alert", "Do you want to delete this card?", "YES", "NO");
                    if (delete)
                    {
                        await Database.DeleteCardAsync(UserInfo.EMail, int.Parse((sender as TextCell).ClassId));
                        CardsItem.Remove(textCell);
                    }
                };
                CardsItem.Add(textCell);
                LoadCards();
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

