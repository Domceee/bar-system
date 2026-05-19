using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Bar> Bars => Set<Bar>();
    public DbSet<BlackjackProfile> BlackjackProfiles => Set<BlackjackProfile>();
    public DbSet<Drink> Drinks => Set<Drink>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<User> Users => Set<User>();
    public DbSet<TasteProfile> TasteProfiles => Set<TasteProfile>();
    public DbSet<TasteAnswer> TasteAnswers => Set<TasteAnswer>();

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<CocktailRecipe> CocktailRecipes => Set<CocktailRecipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<Friend> Friends => Set<Friend>();
    public DbSet<Message> Messages => Set<Message>();

    public DbSet<backend.Models.Route> Routes => Set<backend.Models.Route>();
    public DbSet<BarInRoute> BarsInRoute => Set<BarInRoute>();

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

        mb.Entity<Friend>()
            .HasOne(f => f.Requester)
            .WithMany()
            .HasForeignKey(f => f.RequesterId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Friend>()
            .HasOne(f => f.Addressee)
            .WithMany()
            .HasForeignKey(f => f.AddresseeId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<BarInRoute>()
            .HasOne(b => b.Route)
            .WithMany(r => r.Bars)
            .HasForeignKey(b => b.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<BarInRoute>()
            .HasOne(b => b.Bar)
            .WithMany()
            .HasForeignKey(b => b.BarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
