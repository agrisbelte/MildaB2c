﻿using Microsoft.Identity.Client;
using Microsoft.UI.Xaml;
using Milda.B2C.Maui.MSALClient;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Milda.B2C.Maui.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        //public App()
        //{
        //    this.InitializeComponent();
        //}

        //protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();



        public App()
        {
            this.InitializeComponent();

            // configure redirect URI for your application
            PlatformConfig.Instance.RedirectUri = "http://localhost:60808";

            // Initialize MSAL
            IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            var app = Milda.B2C.Maui.App.Current;
            PlatformConfig.Instance.ParentWindow = ((MauiWinUIWindow)app.Windows[0].Handler.PlatformView).WindowHandle;
        }
    }

}
