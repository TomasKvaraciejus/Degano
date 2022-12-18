using Degano.SqliteDb;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Degano.Views
{
    [ExcludeFromCodeCoverage]
    public partial class SettingsPage : ContentPage
    {
        SettingsPage_Brands settingsPageBrands;
        SettingsPage_MyAccount settingsPageMyAccount;
        CardPage CardPage;
        public Action<bool> UpdateTraffic;

        private static ObservableCollection<KeyValuePair<string, bool>> shownBrands = new ObservableCollection<KeyValuePair<string, bool>>();

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
            GasStationSelect.ItemsSource = GasStation.selectedGasStations;

            ResetTappableLists();
        }

        private void OnDistanceChange(object sender, EventArgs e)
        {
            GasStation.distMax = DistanceSlider.Value;
        }

        private void OnFuelTypeChange(object sender, EventArgs e)
        {
            RadioButton s = (RadioButton)sender;
            GasStation.selectedType = s.Content.ToString();
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

        private async void OnTrafficChanged(object sender, CheckedChangedEventArgs e)
        {
            UpdateTraffic(e.Value);
        }

        private async void OnFilterByBrandsTapped(object sender, EventArgs e)
        {
            if(ToggleFilterByBrands.Text == "Open filter by brands")
            {
                ToggleFilterByBrands.Text = "Close filter by brands";
                GasStationSelect.IsVisible = true;
            }
            else
            {
                ToggleFilterByBrands.Text = "Open filter by brands";
                GasStationSelect.IsVisible = false;
            }
        }

        private async void OnToggleDistanceSliderTapped(object sender, EventArgs e)
        {
            if (ToggleDistanceSlider.Text == "Open search range")
            {
                ToggleDistanceSlider.Text = "Close search range";
                SearchRangeSelect.IsVisible = true;
            }
            else
            {
                ToggleDistanceSlider.Text = "Open search range";
                SearchRangeSelect.IsVisible = false;
            }
        }

        private async void OnToggleFuelTypeTapped(object sender, EventArgs e)
        {
            if (ToggleFuelType.Text == "Open fuel type selection")
            {
                ToggleFuelType.Text = "Close fuel type selection";
                SelectFuelType.IsVisible = true;
            }
            else
            {
                ToggleFuelType.Text = "Open fuel type selection";
                SelectFuelType.IsVisible = false;
            }
        }

        private async void ResetTappableLists()
        {
            ToggleFilterByBrands.Text = ToggleFilterByBrands.Text = "Open filter by brands";
            GasStationSelect.IsVisible = false;

            ToggleDistanceSlider.Text = "Open search range";
            SearchRangeSelect.IsVisible = false;

            ToggleFuelType.Text = "Open fuel type selection";
            SelectFuelType.IsVisible = false;
        }
        private async void OnGasStationToggle(object sender, CheckedChangedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var kvp = (KeyValuePair<string, bool>)cb.BindingContext;

            GasStation.selectedGasStations[kvp.Key] = e.Value;
        }

        protected override bool OnBackButtonPressed()
        {
            MainPage.UpdateShownGasStations();
            ResetTappableLists();
            Navigation.PopAsync();

            return true;
        }
    }
}
