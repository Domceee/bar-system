using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarController(IBarService barService, GoogleMapsInterface googleMaps, AppDbContext db, IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBars() =>
        Ok(await barService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBar(int id)
    {
        var bar = await barService.GetByIdAsync(id);
        return bar is null ? NotFound() : Ok(bar);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBar([FromBody] CreateBarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var bar = await barService.CreateAsync(dto);
        return Ok(bar);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBar(int id, [FromBody] UpdateBarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var bar = await barService.UpdateAsync(id, dto);
        return bar is null ? NotFound() : Ok(bar);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBar(int id)
    {
        var deleted = await barService.DeleteAsync(id);
        return deleted ? Ok() : NotFound();
    }

    [HttpPost("within-distance")]
    public async Task<IActionResult> requestBarsWithinDistance([FromBody] BarsWithinDistanceRequest req)
    {
        var distance = req.DistanceMeters;
        var response = await googleMaps.requestBarsWithinDistance(req.Lat, req.Lon, distance);

        if (!analyzeCode(response))
        {
            // (13-14)
            return BadRequest(new { status = response.Status });
        }

        var bars = (await barService.findBarsWithSameCoordinates()).ToList();

        if (response.Bars.Count < 20)
        {
            var iterations = 0;
            while (iterations < 10 && response.Bars.Count < 20)
            {
                distance = increaseDistance(distance);
                response = await googleMaps.requestBarsWithinDistance(req.Lat, req.Lon, distance);
                bars = (await barService.findBarsWithSameCoordinates()).ToList();
                iterations++;
            }

            if (iterations == 10)
            {
                // (24-25)
                return BadRequest(new { status = "MAX_ITERATIONS_REACHED" });
            }
        }

        var tasteProfile = await db.TasteProfiles
            .Include(p => p.Answers)
            .Where(p => p.UserId == req.UserId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        var sortedByName = sortBarsByName(bars);
        var scoredBars = new List<(Bar Bar, int Priority)>();

        foreach (var bar in sortedByName)
        {
            var current = bar.SelectBar();
            var priority = scoreBar(current, tasteProfile, req.Lat, req.Lon);
            scoredBars.Add((current, priority));
        }

        var ranked = sortBarsByCalculatedRating(scoredBars);

        var dbBars = ranked
            .Where(s => DistanceKm(req.Lat, req.Lon, s.Bar.XCoord, s.Bar.YCoord) * 1000 <= req.DistanceMeters)
            .Select(s => new DbBarDto(s.Bar.Id, s.Bar.Name, s.Bar.XCoord, s.Bar.YCoord, s.Bar.Rating, s.Bar.Design.ToString(), s.Priority))
            .ToList();

        // response within-distance
        return Ok(response with { DbBars = dbBars });
    }

    [HttpPost("fitting-bars")]
    public async Task<IActionResult> requestFittingBars([FromBody] RequestFittingBarsDto dto)
    {
        var allBars = await db.Bars.Include(b => b.Drinks).ToListAsync();
        var combinedList = new List<int>();

        foreach (var userId in dto.UserIds)
        {
            var profile = await userService.getUserTasteProfile(userId);

            if (profile is null) continue;

            var userList = allBars
                .Select(b => (Bar: b, Score: scoreBar(b, profile, 54.9, 23.9)))
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Take(8)
                .Select(x => x.Bar.Id)
                .ToList();

            combinedList.AddRange(userList);
        }

        var selectedBarIds = aggregateBarLists(combinedList);
        if (!selectedBarIds.Any())
            selectedBarIds = allBars.Take(4).Select(b => b.Id).ToList();

        var route = new backend.Models.Route { Status = RouteStatus.Draft };
        db.Routes.Add(route);
        await db.SaveChangesAsync();

        for (var i = 0; i < selectedBarIds.Count; i++)
            db.BarsInRoute.Add(new BarInRoute
            {
                RouteId = route.Id,
                BarId = selectedBarIds[i],
                Order = i + 1,
                IsLast = i == selectedBarIds.Count - 1
            });

        await db.SaveChangesAsync();

        var result = await db.BarsInRoute
            .Include(bir => bir.Bar)
            .Where(bir => bir.RouteId == route.Id)
            .OrderBy(bir => bir.Order)
            .Select(bir => new BarInRouteDto(bir.Id, bir.BarId, bir.Bar.Name, bir.Bar.Address, bir.Order, bir.IsLast, bir.IsCompleted))
            .ToListAsync();

        return Ok(new RouteDto(route.Id, route.Status.ToString(), result));
    }

    [HttpGet("{barId:int}/details")]
    public async Task<IActionResult> fetchBarDetails(int barId)
    {
        var bar = await db.Bars.Include(b => b.Drinks).FirstOrDefaultAsync(b => b.Id == barId);
        return bar is null ? NotFound() : Ok(bar);
    }

    private static List<int> aggregateBarLists(List<int> combinedList) =>
        selectMostRepeatingBars(combinedList);

    private static List<int> selectMostRepeatingBars(List<int> combinedList) =>
        combinedList
            .GroupBy(id => id)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key)
            .ToList();

    private static bool analyzeCode(BarsWithinDistanceResponse response) => response.Status == "OK";

    private static double increaseDistance(double currentDistance) => currentDistance + 1000;

    private static List<Bar> sortBarsByName(IEnumerable<Bar> bars) =>
        bars.OrderBy(b => b.Name).ToList();

    private static List<(Bar Bar, int Priority)> sortBarsByCalculatedRating(IEnumerable<(Bar Bar, int Priority)> scored) =>
        scored
            .OrderByDescending(s => s.Priority)
            .ThenByDescending(s => s.Bar.Rating)
            .ToList();

    private static int increasePriority(int priority) => priority + 1;

    private static int scoreBar(Bar bar, TasteProfile? profile, double userLat, double userLon)
    {
        if (profile is null) return 0;

        var answers = profile.Answers.ToDictionary(a => a.QuestionKey, a => a.Answer);
        var priority = 0;

        if (answers.TryGetValue("budget", out var budget) && hasDrinkInBudget(bar, budget))
            priority = increasePriority(priority);

        if (answers.TryGetValue("drink_type", out var drinkType) && hasDrinkType(bar, drinkType))
            priority = increasePriority(priority);

        if (answers.TryGetValue("flavor_profile", out var flavor) && hasDrinkFlavor(bar, flavor))
            priority = increasePriority(priority);

        if (answers.TryGetValue("bar_distance", out var distancePref) && fitsDistance(bar, distancePref, userLat, userLon))
            priority = increasePriority(priority);

        if (answers.TryGetValue("bar_rating", out var ratingPref) && meetsRatingMinimum(bar, ratingPref))
            priority = increasePriority(priority);

        if (answers.TryGetValue("bar_design", out var designPref) && matchesDesign(bar, designPref))
            priority = increasePriority(priority);

        if (answers.TryGetValue("flavor_balance", out var balance) && hasDrinkFlavorBalance(bar, balance))
            priority = increasePriority(priority);

        if (answers.TryGetValue("drink_strength", out var strength) && hasDrinkStrength(bar, strength))
            priority = increasePriority(priority);

        if (answers.TryGetValue("atmosphere", out var atmosphere) && matchesAtmosphere(bar, atmosphere))
            priority = increasePriority(priority);

        if (answers.TryGetValue("seating", out var seating) && matchesSeating(bar, seating))
            priority = increasePriority(priority);

        return priority;
    }

    private static bool hasDrinkFlavorBalance(Bar bar, string balanceAnswer)
    {
        if (!Enum.TryParse<DrinkFlavorBalance>(balanceAnswer, true, out var balance)) return false;
        return bar.Drinks.Any(d => d.FlavorBalance == balance);
    }

    private static bool hasDrinkStrength(Bar bar, string strengthAnswer)
    {
        if (!Enum.TryParse<DrinkStrength>(strengthAnswer, true, out var strength)) return false;
        return bar.Drinks.Any(d => d.Strength == strength);
    }

    private static bool matchesAtmosphere(Bar bar, string atmosphereAnswer)
    {
        var atmosphere = atmosphereAnswer switch
        {
            "Quiet & relaxed" => (BarAtmosphere?)BarAtmosphere.Quiet,
            "Lively chatter" => BarAtmosphere.Lively,
            "Music & dancing" => BarAtmosphere.Music,
            "Party energy" => BarAtmosphere.Party,
            _ => null
        };
        return atmosphere.HasValue && bar.Atmosphere == atmosphere.Value;
    }

    private static bool matchesSeating(Bar bar, string seatingAnswer)
    {
        var seating = seatingAnswer switch
        {
            "Indoor" => (BarSeating?)BarSeating.Indoor,
            "Outdoor patio" => BarSeating.Outdoor,
            "Bar counter" => BarSeating.Counter,
            "No preference" => BarSeating.NoPreference,
            _ => null
        };
        if (!seating.HasValue) return false;
        if (seating.Value == BarSeating.NoPreference) return true;
        return bar.Seating == seating.Value;
    }

    private static bool hasDrinkInBudget(Bar bar, string budgetAnswer)
    {
        var (min, max) = budgetAnswer switch
        {
            "$1-5" => (1m, 5m),
            "$5-10" => (5m, 10m),
            "$10-15" => (10m, 15m),
            "$15-25" => (15m, 25m),
            "$25+" => (25m, decimal.MaxValue),
            _ => (0m, decimal.MaxValue)
        };
        return bar.Drinks.Any(d => d.Price >= min && d.Price <= max);
    }

    private static bool hasDrinkType(Bar bar, string typeAnswer)
    {
        var normalized = typeAnswer.TrimEnd('s');
        if (!Enum.TryParse<DrinkType>(normalized, true, out var type)) return false;
        return bar.Drinks.Any(d => d.Type == type);
    }

    private static bool hasDrinkFlavor(Bar bar, string flavorAnswer)
    {
        if (!Enum.TryParse<DrinkFlavor>(flavorAnswer, true, out var flavor)) return false;
        return bar.Drinks.Any(d => d.Flavor == flavor);
    }

    private static bool fitsDistance(Bar bar, string distanceAnswer, double userLat, double userLon)
    {
        var distKm = DistanceKm(userLat, userLon, bar.XCoord, bar.YCoord);
        return distanceAnswer switch
        {
            "Under 1 km" => distKm < 1,
            "1-5 km" => distKm >= 1 && distKm <= 5,
            "5-15 km" => distKm > 5 && distKm <= 15,
            "Any distance" => true,
            _ => true
        };
    }

    private static bool meetsRatingMinimum(Bar bar, string ratingAnswer)
    {
        if (!int.TryParse(ratingAnswer.Split(' ')[0], out var minRating)) return false;
        return bar.Rating >= minRating;
    }

    private static bool matchesDesign(Bar bar, string designAnswer)
    {
        if (!Enum.TryParse<BarDesign>(designAnswer, true, out var design)) return false;
        return bar.Design == design;
    }

    private static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
