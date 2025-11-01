using Weather_API.Models;

namespace Weather_API.Managers.Interfaces;

public interface IWeatherManager
{
    Task<WeatherRecord> GetWeatherByCity(string city);
}
