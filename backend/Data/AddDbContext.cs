using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Bar> Bars => Set<Bar>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<User> Users => Set<User>();
    public DbSet<TasteProfile> TasteProfiles => Set<TasteProfile>();
    public DbSet<TasteAnswer> TasteAnswers => Set<TasteAnswer>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Reservation>().HasMany(r => r.Tables).WithMany(t => t.Reservations);

        mb.Entity<TasteProfile>()
            .HasOne(p => p.User)
            .WithMany(u => u.TasteProfiles)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<TasteAnswer>()
            .HasOne(a => a.TasteProfile)
            .WithMany(p => p.Answers)
            .HasForeignKey(a => a.TasteProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
