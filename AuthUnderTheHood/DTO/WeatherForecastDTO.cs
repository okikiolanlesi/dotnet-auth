namespace AuthUnderTheHood.DTO;

public class WeatherForecastDTO
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; } = string.Empty;
    public int TemperatureF { get; set; }

}
