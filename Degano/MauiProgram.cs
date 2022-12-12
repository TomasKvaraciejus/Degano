using Degano.Handlers;
using Degano.SqliteDb;
using Map = Degano.Controls.Map;

namespace Degano;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler(typeof(Map), typeof(MapHandler));
			}).RegisterModels();

		MauiApp app = builder.Build();

        return builder.Build();
	}

    private static MauiAppBuilder RegisterModels(this MauiAppBuilder builder)
    {
        // Main views
        builder.Services.AddSingleton<LandingPage>();
        builder.Services.AddSingleton<Views.SignInPage>();
        builder.Services.AddSingleton<Views.SignUpPage>();
        builder.Services.AddSingleton<Views.MainPage>();
		builder.Services.AddSingleton<Views.SettingsPage>();
        builder.Services.AddSingleton<Views.SettingsPage_Brands>();
		builder.Services.AddSingleton<Views.SettingsPage_MyAccount>();

        // Navigation page
        builder.Services.AddSingleton<NavigationPage>();

        // Validators
        builder.Services.AddSingleton<Helpers.EmailValidator>();
        builder.Services.AddSingleton<Helpers.PasswordValidator>();

		// Database
		builder.Services.AddSingleton<SqliteDb.SqliteDatabase>();
		builder.Services.AddSingleton<UserResult>();

		builder.Services.AddSingleton<List<GasStation>>();
        
        return builder;
    }
}
