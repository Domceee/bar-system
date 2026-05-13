using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Services;
using backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBarService, BarService>();
builder.Services.AddHttpClient<OpenWeatherInterface>();
builder.Services.AddHttpClient<GoogleMapsInterface>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Bars.Any())
    {
        var bars = new List<Bar>
        {
            new() { Name = "Prohibicija", XCoord = 54.8972, YCoord = 23.8860, Rating = 4.5, Address = "Pelesos g. 5, Kaunas", OpenTime = new TimeOnly(22, 0), CloseTime = new TimeOnly(23, 0) },
            new() { Name = "Pabo Latino", XCoord = 54.9001, YCoord = 23.9100, Rating = 3.8, Address = "Vilniaus g. 24, Kaunas", OpenTime = new TimeOnly(21, 0), CloseTime = new TimeOnly(23, 0) },
        };
        db.Bars.AddRange(bars);
        db.SaveChanges();

        db.Tables.AddRange(
            new Table { BarId = bars[0].Id, SeatCount = 2, Status = "available", IsOutside = false },
            new Table { BarId = bars[0].Id, SeatCount = 4, Status = "available", IsOutside = false },
            new Table { BarId = bars[0].Id, SeatCount = 4, Status = "available", IsOutside = true },
            new Table { BarId = bars[1].Id, SeatCount = 2, Status = "available", IsOutside = false },
            new Table { BarId = bars[1].Id, SeatCount = 6, Status = "available", IsOutside = false }
        );
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
