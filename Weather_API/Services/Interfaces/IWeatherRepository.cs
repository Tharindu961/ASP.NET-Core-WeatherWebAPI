using Weather_API.Models;

namespace Weather_API.Services.Interfaces;

public interface IWeatherRepository
{
    Task<WeatherRecord?> GetByCityAsync(string city);
    Task<int> InsertAsync(WeatherRecord record);
    Task<int> UpdateAsync(WeatherRecord record);
}
