using Degano.Helpers;

namespace Degano.Views
{
    public partial class SignUpPage : ContentPage
    {
        private EmailValidator emailValidator { get; } = new EmailValidator();
        private PasswordValidator passwordValidator { get; } = new PasswordValidator();
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void OnEmailTextChange(object sender, EventArgs e)
        {
            UserInfo.EMail = ((Entry)sender).Text;
            if (emailValidator.IsEmailValid(((Entry)sender).Text))
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
            if (passwordValidator.IsPasswordValid(((Entry)sender).Text))
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

        private void OnSubmitClick(object sender, EventArgs e) => MainPage.InitializeMainPage(this);

        private void CheckIsValid()
        {
            if (emailValidator.isValid && passwordValidator.isValid)
                Submit.IsEnabled = true;
            else
                Submit.IsEnabled = false;
        }
    }
}
