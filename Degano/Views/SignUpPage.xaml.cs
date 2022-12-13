using Degano.Helpers;
using Degano.SqliteDb;
using System.Diagnostics.CodeAnalysis;

namespace Degano.Views
{
    [ExcludeFromCodeCoverage]
    public partial class SignUpPage : ContentPage
    {
        SqliteDatabase database;
        EmailValidator emailValidator;
        PasswordValidator passwordValidator;
        MainPage mainPage;
        UserResult userResult;
        public SignUpPage(EmailValidator _emailValidator, PasswordValidator _passwordValidator, SqliteDatabase _database, MainPage _mainPage, UserResult _userResult)
        {
            InitializeComponent();
            emailValidator = _emailValidator;
            passwordValidator = _passwordValidator;
            database = _database;
            mainPage = _mainPage;
            userResult = _userResult;
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
                userResult.Email = EmailEntry.Text;
                userResult.Password = PasswordEntry.Text;
                await database.SaveItemAsync(userResult);
                mainPage.Title = userResult.Email;
                await UserPermissions.GetPermissions();
                await Navigation.PushAsync(mainPage);
                return;
            }
            await DisplayAlert("Error", "User already exists", "OK");
            return;

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
