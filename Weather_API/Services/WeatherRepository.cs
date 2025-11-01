using WeatherApi.Services.Interfaces;

namespace WeatherApi.Services;

public class WeatherRepository : IWeatherRepository
{
    private readonly string _connectionString;

    public WeatherRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
}
