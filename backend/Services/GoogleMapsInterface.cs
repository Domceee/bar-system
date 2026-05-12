using System.Text.Json;
using backend.Models;

namespace backend.Services;

public class GoogleMapsInterface(HttpClient http, IConfiguration config)
{
    public async Task<Bar?> FindNearestBar(double userLat, double userLon, List<Bar> candidates)
    {
        if (candidates.Count == 0) return null;

        var key = config["GoogleMaps:ApiKey"];
        var origins = $"{userLat},{userLon}";
        var destinations = string.Join("|", candidates.Select(b => $"{b.XCoord},{b.YCoord}"));
        var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origins}&destinations={destinations}&key={key}";

        var response = await http.GetFromJsonAsync<JsonElement>(url);
        var elements = response.GetProperty("rows")[0].GetProperty("elements");

        var minIndex = 0;
        var minDistance = int.MaxValue;
        for (int i = 0; i < elements.GetArrayLength(); i++)
        {
            var element = elements[i];
            if (element.GetProperty("status").GetString() != "OK") continue;
            var distance = element.GetProperty("distance").GetProperty("value").GetInt32();
            if (distance < minDistance) { minDistance = distance; minIndex = i; }
        }

        return candidates[minIndex];
    }
}
