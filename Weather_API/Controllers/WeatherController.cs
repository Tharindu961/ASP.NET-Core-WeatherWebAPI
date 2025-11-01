using Microsoft.AspNetCore.Mvc;
using Weather_API.Managers.Interfaces;

namespace Weather_API.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherManager _manager;

    public WeatherController(IWeatherManager manager)
    {
        _manager = manager;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetWeather(string city)
    {
        var result = await _manager.GetWeatherByCityAsync(city);
        return Ok(result);
    }
}
