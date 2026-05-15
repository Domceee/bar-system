using System.Text.Json;
using backend.DTOs;
using backend.Models;

namespace backend.Services;

public class GoogleMapsInterface(HttpClient http, IConfiguration config)
{
    public async Task<BarsWithinDistanceResponse> requestBarsWithinDistance(double lat, double lon, double radiusMeters)
    {
        var key = config["GoogleMaps:ApiKey"];

        if (string.IsNullOrWhiteSpace(key))
        {
            return MockResponse(lat, lon, radiusMeters);
        }

        var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lon}&radius={radiusMeters}&type=bar&key={key}";

        var raw = await http.GetFromJsonAsync<JsonElement>(url);
        var status = raw.TryGetProperty("status", out var s) ? s.GetString() ?? "UNKNOWN_ERROR" : "UNKNOWN_ERROR";

        var bars = new List<NearbyBar>();
        if (raw.TryGetProperty("results", out var results) && results.ValueKind == JsonValueKind.Array)
        {
            foreach (var place in results.EnumerateArray())
            {
                var name = place.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                var location = place.GetProperty("geometry").GetProperty("location");
                var placeLat = location.GetProperty("lat").GetDouble();
                var placeLon = location.GetProperty("lng").GetDouble();
                var placeId = place.TryGetProperty("place_id", out var pid) ? pid.GetString() ?? "" : "";
                double? rating = place.TryGetProperty("rating", out var r) ? r.GetDouble() : null;
                bars.Add(new NearbyBar(name, placeLat, placeLon, placeId, rating));
            }
        }

        return new BarsWithinDistanceResponse(status, bars, []);
    }

    private static BarsWithinDistanceResponse MockResponse(double lat, double lon, double radiusMeters)
    {
        var radiusDeg = radiusMeters / 111_000;
        var rng = new Random(42);
        var bars = Enumerable.Range(0, 25)
            .Select(i => new NearbyBar(
                $"Mock Bar {i + 1}",
                lat + (rng.NextDouble() - 0.5) * radiusDeg * 2,
                lon + (rng.NextDouble() - 0.5) * radiusDeg * 2,
                $"mock-place-{i}",
                3.0 + rng.NextDouble() * 2.0))
            .ToList();
        return new BarsWithinDistanceResponse("OK", bars, []);
    }

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
