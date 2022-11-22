using Degano.Views;

namespace Degano
{
	public partial class LandingPage : ContentPage
	{
		SignInPage signInPage;

		SignUpPage signUpPage;

		MainPage mainPage;
        public LandingPage(SignUpPage _signUpPage, SignInPage _signInPage, MainPage _mainPage)
        {
			InitializeComponent();
            signInPage = _signInPage;
            signUpPage = _signUpPage;
            mainPage = _mainPage;
        }

		private void OnSignInClick(object sender, EventArgs e) => Navigation.PushAsync(signInPage);

		private void OnSignUpClick(object sender, EventArgs e) => Navigation.PushAsync(signUpPage);

		private void OnContinueWithoutSignInClick(object sender, EventArgs e) => Navigation.PushAsync(mainPage);
	}
}
