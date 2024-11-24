using Foundation;
using Microsoft.Identity.Client;
using Milda.B2C.Maui.MSALClient;
using UIKit;

namespace Milda.B2C.Maui
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Initialize MSAL and platformConfig is set
            IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
