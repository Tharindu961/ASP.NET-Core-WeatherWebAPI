using Microsoft.AspNetCore.Mvc;
using Weather_API.Managers.Interfaces;
using Weather_API.Services.Interfaces;

namespace Weather_API.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherManager _manager;
    private readonly IWeatherRepository _repository;

    public WeatherController(IWeatherManager manager, IWeatherRepository repository)
    {
        _manager = manager;
        _repository = repository;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetWeather(string city)
    {
        var result = await _manager.GetWeatherByCityAsync(city);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repository.GetAllAsync();
        return Ok(data);
    }
}
