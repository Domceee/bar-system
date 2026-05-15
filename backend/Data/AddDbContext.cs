using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Bar> Bars => Set<Bar>();
<<<<<<< HEAD
    public DbSet<BlackjackProfile> BlackjackProfiles => Set<BlackjackProfile>();
}
=======
    public DbSet<Drink> Drinks => Set<Drink>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<User> Users => Set<User>();
    public DbSet<TasteProfile> TasteProfiles => Set<TasteProfile>();
    public DbSet<TasteAnswer> TasteAnswers => Set<TasteAnswer>();

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<CocktailRecipe> CocktailRecipes => Set<CocktailRecipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Reservation>().HasMany(r => r.Tables).WithMany(t => t.Reservations);

        mb.Entity<Drink>()
            .HasOne(d => d.Bar)
            .WithMany(b => b.Drinks)
            .HasForeignKey(d => d.BarId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Drink>().Property(d => d.Price).HasPrecision(10, 2);

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

        mb.Entity<Ingredient>().Property(i => i.Price).HasPrecision(10, 2);

        mb.Entity<CocktailRecipe>()
            .HasOne(r => r.Author)
            .WithMany()
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany()
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
>>>>>>> 4e2a57b0275e3df79edbdf0ef911869d606f1b66
