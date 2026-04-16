using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class BarService(AppDbContext db) : IBarService
{
    public async Task<IEnumerable<Bar>> GetAllAsync() =>
        await db.Bars.ToListAsync();

    public async Task<Bar?> GetByIdAsync(int id) =>
        await db.Bars.FindAsync(id);

    public async Task<Bar> CreateAsync(CreateBarDto dto)
    {
        var bar = new Bar { Name = dto.Name, XCoord = dto.XCoord, YCoord = dto.YCoord };
        db.Bars.Add(bar);
        await db.SaveChangesAsync();
        return bar;
    }

    public async Task<Bar?> UpdateAsync(int id, UpdateBarDto dto)
    {
        var bar = await db.Bars.FindAsync(id);
        if (bar is null) return null;

        bar.Name = dto.Name;
        bar.XCoord = dto.XCoord;
        bar.YCoord = dto.YCoord;
        await db.SaveChangesAsync();
        return bar;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var bar = await db.Bars.FindAsync(id);
        if (bar is null) return false;

        db.Bars.Remove(bar);
        await db.SaveChangesAsync();
        return true;
    }
}
