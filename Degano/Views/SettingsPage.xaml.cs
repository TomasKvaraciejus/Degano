using Degano.SqliteDb;

namespace Degano.Views
{
    public partial class SettingsPage : ContentPage
    {
        SettingsPage_Brands settingsPageBrands;
        SettingsPage_MyAccount settingsPageMyAccount;

        UserResult userResult;
        
        SqliteDatabase database;
        public SettingsPage(SqliteDatabase db, UserResult _userResult, SettingsPage_Brands _settingsPageBrands, SettingsPage_MyAccount _settingsPageMyAccount)
        {
            InitializeComponent();
            database = db;
            userResult = _userResult;
            settingsPageBrands = _settingsPageBrands;
            settingsPageMyAccount = _settingsPageMyAccount;
            DistanceSlider.Value = GasStation.distMax;
        }

        private void OnDistanceChange(object sender, EventArgs e)
        {
            GasStation.distMax = DistanceSlider.Value;
        }

        private async void OnMyAccount(object sender, EventArgs e)
        {
            settingsPageMyAccount.OnPageEntry();
            await Navigation.PushAsync(settingsPageMyAccount);
        }

        private async void OnFilterBrands(object sender, EventArgs e)
        {
            await Navigation.PushAsync(settingsPageBrands); 
        }

        private async void OnAddCard(object sender, EventArgs e)
        {
            Cards card = new Cards();
            card.CardName = "viada";
            card.Discount = 10;
            card.Email = UserInfo.EMail;
            userResult.Email = UserInfo.EMail;
            userResult.Password = UserInfo.Password;
            var c = await database.InsertCardAsync(card);
        }

        private async void OnShowCards(object sender, EventArgs e)
        {
            var cards = await database.GetCardsAsync(UserInfo.EMail);
            foreach (var card in cards)
            {
                await DisplayAlert("Card", $"Gas station - {card.CardName}\nDiscount - {card.Discount}%", "OK");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            MainPage.UpdateShownGasStations();
            Navigation.PopAsync();

            return true;
        }
    }
}
