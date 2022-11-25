using Degano.SqliteDb;

namespace Degano.Views
{
    public partial class SettingsPage : ContentPage
    {
        UserResult userResult;
        
        SqliteDatabase database;
        public SettingsPage(SqliteDatabase db, UserResult _userResult)
        {
            InitializeComponent();
            GasStationSelect.ItemsSource = GasStation.selectedGasStations;
            database = db;
            userResult = _userResult;
        }

        protected override bool OnBackButtonPressed()
        {
            MainPage.UpdateShownGasStations();
            System.Diagnostics.Debug.WriteLine("aaa");
            Navigation.PopAsync();

            return true;
        }

        private async void OnAddCardButton(object sender, EventArgs e)
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

        private async void OnGasStationToggle(object sender, CheckedChangedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var kvp = (KeyValuePair<string, bool>)cb.BindingContext;

            GasStation.selectedGasStations[kvp.Key] = e.Value;
        }
    }
}
