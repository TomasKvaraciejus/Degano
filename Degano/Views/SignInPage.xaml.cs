using Degano.Helpers;
using Degano.SqliteDb;

namespace Degano.Views
{
    public partial class SignInPage : ContentPage
    {
        SqliteDatabase database;
        EmailValidator emailValidator;
        PasswordValidator passwordValidator;
        MainPage mainPage;
        public SignInPage(EmailValidator _emailValidator, PasswordValidator _passwordValidator, SqliteDatabase _database, MainPage _mainPage)
        {
            InitializeComponent();
            emailValidator = _emailValidator;
            passwordValidator = _passwordValidator;
            database = _database;
            mainPage = _mainPage;
        }

        private void OnEmailTextChange(object sender, EventArgs e)
        {
            UserInfo.EMail = ((Entry)sender).Text;
            if (string.IsNullOrEmpty(((Entry)sender).Text) || emailValidator.IsEmailValid(((Entry)sender).Text))
            {
                EmailLabel.IsVisible = false;
            }
            else
            {
                EmailLabel.IsVisible = true;
            }
            CheckIsValid();
        }

        private void OnPasswordTextChange(object sender, EventArgs e)
        {
            UserInfo.Password = ((Entry)sender).Text;
            if (string.IsNullOrEmpty(((Entry)sender).Text) || passwordValidator.IsPasswordValid(((Entry)sender).Text))
            {
                PasswordLabel.IsVisible = false;
            }
            else
            {
                PasswordLabel.Text = passwordValidator.ErrorMessage;
                PasswordLabel.IsVisible = true;
            }
            CheckIsValid();
        }

        private async void OnSubmitClick(object sender, EventArgs e)
        {
            if (await database.GetItemAsync(EmailEntry.Text) is null)
            {
                await DisplayAlert("Error", "User does not exist", "OK");
            }
            else
            {
                var user = await database.GetItemAsync(EmailEntry.Text);
                if (user.Password == PasswordEntry.Text)
                {
                    
                    mainPage.Title = user.Email;
                    await UserPermissions.GetPermissions();
                    await Navigation.PushAsync(mainPage);
                    return;
                }
                await DisplayAlert("Error", "Password is incorrect", "OK");
            }
        }

        private void CheckIsValid()
        {
            if (emailValidator.isValid && passwordValidator.isValid)
                Submit.IsEnabled = true;
            else
                Submit.IsEnabled = false;
        }
    }
}
