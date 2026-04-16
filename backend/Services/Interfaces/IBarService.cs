using backend.DTOs;
using backend.Models;

namespace backend.Services.Interfaces;

public interface IBarService
{
    Task<IEnumerable<Bar>> GetAllAsync();
    Task<Bar?> GetByIdAsync(int id);
    Task<Bar> CreateAsync(CreateBarDto dto);
    Task<Bar?> UpdateAsync(int id, UpdateBarDto dto);
    Task<bool> DeleteAsync(int id);
}
