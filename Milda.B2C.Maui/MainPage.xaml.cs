using System.Diagnostics;
using Microsoft.Identity.Client;
using Milda.B2C.Maui.MSALClient;

namespace Milda.B2C.Maui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            IAccount cachedUserAccount =
                PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache().Result;

            _ = Dispatcher.DispatchAsync(async () =>
            {
                if (cachedUserAccount == null)
                {
                    SignInButton.IsEnabled = true;
                }
                else
                {
                    await Shell.Current.GoToAsync("ScopePage");
                }
            });
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void OnSignInClicked(object? sender, EventArgs e)
        {
            try
            {
                await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
                await Shell.Current.GoToAsync("ScopePage");
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"Authentication failed: {ex.Message}");
            }
        }
    }

}
