using Microsoft.Identity.Client;
using Milda.B2C.Maui.MSALClient;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using System.Text.Json;

namespace Milda.B2C.Maui.Pages;

public partial class WheatherPage : ContentPage
{
    public WheatherPage(WeatherViewModel viewModel)
	{
        InitializeComponent();
        
        BindingContext = viewModel;
        viewModel.LoadDataAsync().SafeFireAndForget();
    }
}

public class WeatherViewModel : BindableObject
{
    private ObservableCollection<WeatherForecast> _forecasts;
    public ObservableCollection<WeatherForecast> Forecasts
    {
        get => _forecasts;
        set
        {
            _forecasts = value;
            OnPropertyChanged();
        }
    }

    private readonly WeatherService _weatherService;

    public WeatherViewModel()
    {
        _weatherService = new WeatherService();
        Forecasts = new ObservableCollection<WeatherForecast>();
    }

    public async Task LoadDataAsync()
    {
        var forecasts = await _weatherService.GetWeatherForecastAsync();
        foreach (var forecast in forecasts)
        {
            Forecasts.Add(forecast);
        }
    }
}

public class WeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7142/") // Replace with your API base URL
        };
    }

    public async Task<List<WeatherForecast>> GetWeatherForecastAsync()
    {
        var token = await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("WeatherForecast");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = System.Text.Json.JsonSerializer.Deserialize<List<WeatherForecast>>(json, options);

        return result;
    }
}

public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    public string Summary { get; set; }

    public string Emoji => Summary switch
    {
        "Freezing" => "❄️",
        "Bracing" => "💨",
        "Chilly" => "🧥",
        "Cool" => "🌬️",
        "Mild" => "🌤️",
        "Warm" => "☀️",
        "Balmy" => "🌞",
        "Hot" => "🔥",
        "Sweltering" => "🥵",
        "Scorching" => "🌡️",
        _ => "🌈"
    };
}