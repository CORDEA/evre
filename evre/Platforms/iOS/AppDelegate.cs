using Foundation;
using UIKit;

namespace evre;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }

    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        return true;
    }
}
