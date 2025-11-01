using System.ComponentModel.DataAnnotations;
using Weather_API.Managers.Interfaces;
using Weather_API.Models;
using Weather_API.Services.Interfaces;

namespace Weather_API.Managers;

public class WeatherManager : IWeatherManager
{
    private readonly IWeatherRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherManager(IWeatherRepository repository, IConfiguration config, HttpClient httpClient)
    {
        _repository = repository;
        _httpClient = httpClient;
        _apiKey = config["OpenWeatherApiKey"] ?? throw new Exception("OpenWeather API key is missing in configuration.");
    }

    public async Task<WeatherRecord> GetWeatherByCityAsync(string city)
    {
        var existing = await _repository.GetByCityAsync(city);
        var record = new WeatherRecord();
        if (existing != null && (DateTime.Now - existing.LastUpdated).TotalMinutes < 30)
        {
            record = existing;
        }
        else
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new ValidationException("Please enter valid city name.");
            }
            var apiData = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>();
            if (apiData == null)
            {
                throw new ValidationException("Invalid api response.");
            }

            record.City = apiData.Name;
            record.Country = apiData.Sys.Country;
            record.Temperature = apiData.Main.Temp;
            record.Description = apiData.Weather.FirstOrDefault()?.Description ?? "N/A";
            record.Humidity = apiData.Main.Humidity;
            record.Pressure = apiData.Main.Pressure;
            record.WindSpeed = apiData.Wind.Speed;
            record.Cloudiness = apiData.Clouds.All;
            record.LastUpdated = DateTime.Now;
            if (existing != null)
            {
                record.Id = existing.Id;
                await _repository.UpdateAsync(record);
            }
            else
            {
                await _repository.InsertAsync(record);
            }
        }
        return record;
    }
}
