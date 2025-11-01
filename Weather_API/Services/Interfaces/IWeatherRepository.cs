using Weather_API.Models;

namespace Weather_API.Services.Interfaces;

public interface IWeatherRepository
{
    Task<WeatherRecord?> GetByCity(string city);
    Task<int> Insert(WeatherRecord record);
    Task<int> Update(WeatherRecord record);
    Task<List<WeatherRecord>> GetAll();
}
