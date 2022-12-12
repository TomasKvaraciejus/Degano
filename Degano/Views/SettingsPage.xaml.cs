using Degano.SqliteDb;

namespace Degano.Views
{
    public partial class SettingsPage : ContentPage
    {
        SettingsPage_Brands settingsPageBrands;
        SettingsPage_MyAccount settingsPageMyAccount;
        CardPage CardPage;

        UserResult userResult;
        
        SqliteDatabase database;
        public SettingsPage(SqliteDatabase db, UserResult _userResult, SettingsPage_Brands _settingsPageBrands, SettingsPage_MyAccount _settingsPageMyAccount, CardPage cardPage)
        {
            InitializeComponent();
            database = db;
            userResult = _userResult;
            settingsPageBrands = _settingsPageBrands;
            settingsPageMyAccount = _settingsPageMyAccount;
            DistanceSlider.Value = GasStation.distMax;
            CardPage = cardPage;
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

        private async void OnManageCards(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UserInfo.EMail))
            {
                CardPage.LoadCards();
                await Navigation.PushAsync(CardPage);
            }
            else
            {
                await DisplayAlert("Error", "You must be logged in to manage your cards", "OK");
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
