using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace Degano;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

    public override void StartActivity(Intent intent)
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            intent.RemoveFlags(ActivityFlags.LaunchAdjacent);

        base.StartActivity(intent);
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
