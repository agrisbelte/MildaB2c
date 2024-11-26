using Microsoft.Identity.Client;
using Milda.B2C.Maui.MSALClient;

namespace Milda.B2C.Maui.Pages;

public partial class ScopePage : ContentPage
{
    public IEnumerable<string> AccessTokenScopes { get; set; } = new string[] { "No scopes found in access token" };

    public ScopePage()
	{
        BindingContext = this;
        InitializeComponent();
        _ = SetViewDataAsync();
    }

    // Define the bindable property
    public static readonly BindableProperty UserNameProperty =
        BindableProperty.Create(
            nameof(UserName),
            typeof(string),
            typeof(ScopePage),
            default,
            BindingMode.TwoWay,
            propertyChanged: UserNameChnaged);

    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    private static void UserNameChnaged(BindableObject bindable, object oldvalue, object newvalue)
    {
        
    }


    // Define the bindable property
    public static readonly BindableProperty TokenProperty =
        BindableProperty.Create(
            nameof(Token),
            typeof(string),
            typeof(ScopePage),
            default,
            BindingMode.TwoWay,
            propertyChanged: TokenChnaged);

    public string Token
    {
        get => (string)GetValue(TokenProperty);
        set => SetValue(TokenProperty, value);
    }

    private static void TokenChnaged(BindableObject bindable, object oldvalue, object newvalue)
    {
    }


    private async Task SetViewDataAsync()
    {
        try
        {
            var token = await PublicClientSingleton.Instance.AcquireTokenSilentAsync();

            ExpiresAt.Text = PublicClientSingleton.Instance.MSALClientHelper.AuthResult.ExpiresOn.ToLocalTime().ToString();
            AccessTokenScopes = PublicClientSingleton.Instance.MSALClientHelper.AuthResult.Scopes
                .Select(s => s.Split("/").Last());

            Scopes.ItemsSource = AccessTokenScopes;


            var account = PublicClientSingleton.Instance?.MSALClientHelper?.AuthResult?.Account;

            if (account != null)
            {
                UserName = account.Username;
                Token = PublicClientSingleton.Instance?.MSALClientHelper?.AuthResult.AccessToken;
            }

        }

        catch (MsalUiRequiredException)
        {
            await Shell.Current.GoToAsync(nameof(ScopePage));
        }
    }

    protected override bool OnBackButtonPressed() { return true; }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        await PublicClientSingleton.Instance.SignOutAsync().ContinueWith((t) =>
        {
            return Task.CompletedTask;
        });

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    private async void GetForecast(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WheatherPage));
    }
}