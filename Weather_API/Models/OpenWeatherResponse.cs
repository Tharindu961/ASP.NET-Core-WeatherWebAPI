namespace Weather_API.Models;

public class OpenWeatherResponse
{
    public string Name { get; set; } = "";
    public Sys Sys { get; set; } = new Sys();
    public Main Main { get; set; } = new Main();
    public List<Weather> Weather { get; set; } = new List<Weather>();
    public Wind Wind { get; set; } = new Wind();
    public Clouds Clouds { get; set; } = new Clouds();
}

public class Sys { public string Country { get; set; } = ""; }
public class Main { public double Temp { get; set; } public int Humidity { get; set; } public int Pressure { get; set; } }
public class Weather { public string Description { get; set; } = ""; }
public class Wind { public double Speed { get; set; } }
public class Clouds { public int All { get; set; } }
