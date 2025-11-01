namespace WeatherApi.Models;

public class WeatherRecord
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public int Pressure { get; set; }
    public double WindSpeed { get; set; }
    public int Cloudiness { get; set; }
    public DateTime LastUpdated { get; set; }
}
