using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController(AppDbContext db, OpenWeatherInterface openWeather, GoogleMapsInterface googleMap) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> OpenReservations()
    {
        var reservations = await db.Reservations
            .Include(r => r.Bar)
            .Include(r => r.Tables)
            .ToListAsync();

        return Ok(reservations.Select(ToListItemDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await db.Reservations
            .Include(r => r.Bar)
            .Include(r => r.Tables)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation is null) return NotFound();

        var state = reservation.CheckState();

        if (state == "ended")
        {
            db.Reservations.Remove(reservation);
            await db.SaveChangesAsync();

            var all = await db.Reservations.Include(r => r.Bar).Include(r => r.Tables).ToListAsync();
            return Ok(new ReservationResponseDto("deleted", null, all.Select(ToListItemDto).ToList()));
        }

        if (state == "active")
            return BadRequest(new ReservationResponseDto("active", "Cannot delete active reservation", null));

        reservation.ChangeState();
        foreach (var table in reservation.Tables)
            table.FreeTable();
        await db.SaveChangesAsync();

        return Ok(new ReservationResponseDto("cancelled", "Successfully cancelled reservation", null));
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var reservation = await db.Reservations
            .Include(r => r.Tables)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation is null) return NotFound();

        reservation.ChangeState();
        foreach (var table in reservation.Tables)
            table.FreeTable();
        await db.SaveChangesAsync();

        return Ok(new ReservationResponseDto("cancelled", "Successfully cancelled reservation", null));
    }

    [HttpPut("{id}/end")]
    public async Task<IActionResult> EndReservation(int id)
    {
        var reservation = await db.Reservations
            .Include(r => r.Tables)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation is null) return NotFound();

        reservation.ChangeToEnded();
        foreach (var table in reservation.Tables)
            table.FreeTable();
        await db.SaveChangesAsync();

        return Ok(new ReservationResponseDto("ended", "Successfully ended reservation", null));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] ReservationFormDto dto)
    {
        if (!IsValid(dto))
            return BadRequest(new ReservationProposalDto("invalidForm", null, null, null, "Invalid form data"));

        var allBars = await db.Bars.ToListAsync();
        Bar? bar = dto.BarId.HasValue ? allBars.FirstOrDefault(b => b.Id == dto.BarId) : null;

        if (bar is not null)
        {
            bar.SelectBar();

            if (!IsWorking(bar, dto.Date))
            {
                if (!dto.UseNearestBar)
                    return Ok(new ReservationProposalDto("barClosed", null, null, null, null));

                var openBars = allBars.Where(b => IsWorking(b, dto.Date)).ToList();
                var nearestBars = openBars
                    .OrderBy(b => Math.Pow(b.XCoord - dto.UserLat, 2) + Math.Pow(b.YCoord - dto.UserLon, 2))
                    .Take(25)
                    .ToList();
                bar = await googleMap.FindNearestBar(dto.UserLat, dto.UserLon, nearestBars);
                if (bar is null)
                    return Ok(new ReservationProposalDto("error", null, null, null, "Could not find nearest bar"));
            }
        }

        var workingBars = bar is not null
            ? [bar]
            : allBars.Where(b => IsWorking(b, dto.Date)).ToList();

        if (workingBars.Count == 0)
            return Ok(new ReservationProposalDto("noBarsFound", null, null, null, null));

        workingBars = SortByRatingDesc(workingBars);

        while (workingBars.Count > 0)
        {
            var current = workingBars[0];
            current.SelectBar();

            var allTables = await db.Tables.Where(t => t.BarId == current.Id).ToListAsync();
            var freeTables = Table.SelectFreeTables(allTables);
            var suitingTable = FindSuitingTable(freeTables, dto.GuestCount);

            if (suitingTable is not null)
                return Ok(new ReservationProposalDto("found", ToDto(current), [suitingTable.Id], null, null));

            var combinations = CombineTables(freeTables);
            while (combinations.Count > 0)
            {
                var combo = combinations[0];
                if (FindSeatSum(combo) >= dto.GuestCount)
                    return Ok(new ReservationProposalDto("found", ToDto(current), combo.Select(t => t.Id).ToList(), null, null));
                RemoveCombination(combinations, combo);
            }

            RemoveBarFromList(workingBars, current);
        }

        return Ok(new ReservationProposalDto("noBarsFound", null, null, null, null));
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmReservationDto dto)
    {
        var reservation = Reservation.Create(dto.BarId, dto.GuestCount, DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc), "pending");

        var tables = await db.Tables.Where(t => dto.TableIds.Contains(t.Id)).ToListAsync();
        reservation.Tables = tables;

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        reservation.UpdateStatus("confirmed");
        foreach (var table in tables)
            table.Status = "reserved";

        await db.SaveChangesAsync();

        string? weather = null;
        if (IsOutside(reservation))
        {
            var bar = await db.Bars.FindAsync(dto.BarId);
            if (bar is not null)
                weather = await openWeather.GetWeatherForecastByDate(dto.Date, bar.XCoord, bar.YCoord);
        }

        return Ok(new ReservationProposalDto("confirmed", null, dto.TableIds, weather, null));
    }

    private static bool IsValid(ReservationFormDto dto) =>
        dto.GuestCount > 0 && dto.Date > DateTime.Now;

    private static bool IsWorking(Bar bar, DateTime date)
    {
        var time = TimeOnly.FromDateTime(date);
        if (bar.OpenTime <= bar.CloseTime)
            return time >= bar.OpenTime && time <= bar.CloseTime;
        return time >= bar.OpenTime || time <= bar.CloseTime;
    }

    private static List<Bar> SortByRatingDesc(List<Bar> bars) =>
        [.. bars.OrderByDescending(b => b.Rating)];

    private static Table? FindSuitingTable(List<Table> tables, int guestCount) =>
        tables.FirstOrDefault(t => t.SeatCount >= guestCount);

    private static List<List<Table>> CombineTables(List<Table> tables)
    {
        var result = new List<List<Table>>();
        for (int i = 0; i < tables.Count; i++)
            for (int j = i + 1; j < tables.Count; j++)
                result.Add([tables[i], tables[j]]);
        return result;
    }

    private static int FindSeatSum(List<Table> tables) =>
        tables.Sum(t => t.SeatCount);

    private static bool IsOutside(Reservation reservation) =>
        reservation.Tables.Any(t => t.IsOutside);

    private static void RemoveBarFromList(List<Bar> bars, Bar bar) =>
        bars.Remove(bar);

    private static void RemoveCombination(List<List<Table>> combinations, List<Table> combo) =>
        combinations.Remove(combo);

    private static BarDto ToDto(Bar b) =>
        new(b.Id, b.Name, b.XCoord, b.YCoord, b.Rating, b.Address, b.OpenTime, b.CloseTime, b.Design, b.Atmosphere, b.Seating);

    private static ReservationListItemDto ToListItemDto(Reservation r) =>
        new(r.Id, r.BarId, r.Bar.Name, r.GuestCount, r.Date, r.Status, r.Tables.Select(t => t.Id).ToList());
}
