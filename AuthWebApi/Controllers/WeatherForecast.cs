using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebApi.Controllers;


public record WeatherForecastDTO(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

[ApiController]
[Route("weatherforecast")]
[Authorize(Policy = "HR")]
public class WeatherForecastController : ControllerBase
{

    [HttpGet]

    public WeatherForecastDTO[] GetWeatherForecast()
    {
        var summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

        var forecast = Enumerable.Range(1, 5).Select(index =>

           new WeatherForecastDTO
           (
               DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
               Random.Shared.Next(-20, 55),
               summaries[Random.Shared.Next(summaries.Length)]
           ))
           .ToArray();
        return forecast;
    }
}




