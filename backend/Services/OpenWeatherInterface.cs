using System.Text.Json;

namespace backend.Services;

public class OpenWeatherInterface(HttpClient http, IConfiguration config)
{
    public async Task<string> GetWeatherForecastByDate(DateTime date, double lat, double lon)
    {
        var key = config["OpenWeather:ApiKey"];
        var url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={key}&units=metric";
        var response = await http.GetFromJsonAsync<JsonElement>(url);
        var list = response.GetProperty("list");

        var targetTimestamp = ((DateTimeOffset)date).ToUnixTimeSeconds();
        var best = list.EnumerateArray()
            .OrderBy(e => Math.Abs(e.GetProperty("dt").GetInt64() - targetTimestamp))
            .First();

        var description = best.GetProperty("weather")[0].GetProperty("description").GetString() ?? "unknown";
        var temp = best.GetProperty("main").GetProperty("temp").GetDouble();
        return $"{description}, {temp:F1}°C";
    }
}
