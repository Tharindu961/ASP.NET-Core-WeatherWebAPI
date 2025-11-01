using Weather_API.Services.Interfaces;
using Weather_API.Managers.Interfaces;

namespace Weather_API.Managers;

public class WeatherManager : IWeatherManager
{
    private readonly IWeatherRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherManager(IWeatherRepository repository, IConfiguration config)
    {
        _repository = repository;
        _httpClient = new HttpClient();
        _apiKey = config["OpenWeatherApiKey"] ?? throw new Exception("OpenWeather API key is missing in configuration.");
    }

}
