using Behaviors;

namespace Degano.Views
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void OnEmailTextChange(object sender, EventArgs e)
        {
            UserInfo.EMail = ((Entry)sender).Text;
            CheckIsValid();
        }

        private void OnPasswordTextChange(object sender, EventArgs e)
        {
            UserInfo.Password = ((Entry)sender).Text;
            CheckIsValid();
        }

        private void OnSubmitClick(object sender, EventArgs e) => MainPage.InitializeMainPage(this);

        private void CheckIsValid()
        {
            Submit.IsEnabled = EmailEntry.Behaviors.OfType<EmailValidatorBehavior>().First().IsValid && PasswordEntry.Behaviors.OfType<TextValidatorBehavior>().First().IsValid;
            PasswordLabel.IsVisible = !PasswordEntry.Behaviors.OfType<TextValidatorBehavior>().First().IsValid;
            EmailLabel.IsVisible = !EmailEntry.Behaviors.OfType<EmailValidatorBehavior>().First().IsValid;
        }
    }
}
