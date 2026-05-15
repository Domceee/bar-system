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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<OpenWeatherInterface>();
builder.Services.AddHttpClient<GoogleMapsInterface>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any())
    {
        db.Users.Add(new User { Username = "test" });
        db.SaveChanges();
    }

    if (!db.Bars.Any())
    {
        var bars = new List<Bar>
        {
            new() { Name = "Prohibicija",     XCoord = 54.8972, YCoord = 23.8860, Rating = 4.5, Address = "Pelesos g. 5, Kaunas",     OpenTime = new TimeOnly(22, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Vintage },
            new() { Name = "Pabo Latino",     XCoord = 54.9001, YCoord = 23.9100, Rating = 3.8, Address = "Vilniaus g. 24, Kaunas",   OpenTime = new TimeOnly(21, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Modern },
            new() { Name = "Whiskey Den",     XCoord = 54.8985, YCoord = 23.9036, Rating = 4.7, Address = "Laisves al. 10, Kaunas",   OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0),  Design = BarDesign.Cozy },
            new() { Name = "NeonHouse",       XCoord = 54.9050, YCoord = 23.9200, Rating = 4.2, Address = "Savanoriu pr. 1, Kaunas",  OpenTime = new TimeOnly(20, 0), CloseTime = new TimeOnly(4, 0),  Design = BarDesign.Industrial },
            new() { Name = "Sky Lounge",      XCoord = 54.8930, YCoord = 23.8950, Rating = 4.9, Address = "Donelaicio g. 60, Kaunas", OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0),  Design = BarDesign.Luxurious },
            new() { Name = "The Tipsy Owl",   XCoord = 54.9020, YCoord = 23.8800, Rating = 4.3, Address = "Karaliaus Mindaugo pr. 7, Kaunas", OpenTime = new TimeOnly(16, 0), CloseTime = new TimeOnly(0, 0), Design = BarDesign.Cozy },
            new() { Name = "Speakeasy 1920",  XCoord = 54.8960, YCoord = 23.9150, Rating = 4.6, Address = "M. Valanciaus g. 3, Kaunas", OpenTime = new TimeOnly(19, 0), CloseTime = new TimeOnly(3, 0), Design = BarDesign.Vintage },
            new() { Name = "Pulse Club",      XCoord = 54.9100, YCoord = 23.9050, Rating = 3.5, Address = "K. Petrausko g. 15, Kaunas", OpenTime = new TimeOnly(22, 0), CloseTime = new TimeOnly(5, 0), Design = BarDesign.Industrial },
            new() { Name = "Velvet Room",     XCoord = 54.8940, YCoord = 23.8900, Rating = 4.8, Address = "S. Daukanto g. 12, Kaunas", OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Luxurious },
            new() { Name = "Urban Loft",      XCoord = 54.9010, YCoord = 23.8970, Rating = 4.0, Address = "Putvinskio g. 38, Kaunas",  OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0), Design = BarDesign.Modern },
            new() { Name = "The Old Anchor",  XCoord = 54.8995, YCoord = 23.9180, Rating = 3.9, Address = "A. Mickeviciaus g. 5, Kaunas", OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Vintage },
            new() { Name = "Cinnamon",        XCoord = 54.9070, YCoord = 23.8990, Rating = 4.4, Address = "Kestucio g. 22, Kaunas",   OpenTime = new TimeOnly(16, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Cozy },
            new() { Name = "Lumen",           XCoord = 54.8920, YCoord = 23.9100, Rating = 4.1, Address = "Gedimino g. 4, Kaunas",    OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(0, 0), Design = BarDesign.Modern },
            new() { Name = "Metalica",        XCoord = 54.9085, YCoord = 23.9220, Rating = 3.7, Address = "Tunelio g. 9, Kaunas",     OpenTime = new TimeOnly(21, 0), CloseTime = new TimeOnly(5, 0), Design = BarDesign.Industrial },
            new() { Name = "Pearl Lounge",    XCoord = 54.8960, YCoord = 23.8830, Rating = 5.0, Address = "Rotuses a. 1, Kaunas",     OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Luxurious },
            new() { Name = "Tabula Rasa",     XCoord = 54.9030, YCoord = 23.9070, Rating = 4.2, Address = "Birutes g. 17, Kaunas",    OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0), Design = BarDesign.Modern },
            new() { Name = "Hideaway",        XCoord = 54.8975, YCoord = 23.9220, Rating = 4.5, Address = "Aleksoto g. 6, Kaunas",    OpenTime = new TimeOnly(19, 0), CloseTime = new TimeOnly(3, 0), Design = BarDesign.Cozy },
            new() { Name = "Gold & Smoke",    XCoord = 54.9000, YCoord = 23.8920, Rating = 4.7, Address = "Maironio g. 28, Kaunas",   OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Luxurious },
            new() { Name = "The Forge",       XCoord = 54.9055, YCoord = 23.9130, Rating = 3.4, Address = "Linkuvos g. 11, Kaunas",   OpenTime = new TimeOnly(20, 0), CloseTime = new TimeOnly(4, 0), Design = BarDesign.Industrial },
            new() { Name = "Antique Cellar",  XCoord = 54.8940, YCoord = 23.9020, Rating = 4.0, Address = "Vasario 16-osios g. 2, Kaunas", OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(0, 0), Design = BarDesign.Vintage }
        };
        db.Bars.AddRange(bars);
        db.SaveChanges();

        db.Drinks.AddRange(
            new Drink { BarId = bars[0].Id,  Name = "Old Fashioned",       Type = DrinkType.Cocktail, Price = 12.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[0].Id,  Name = "House IPA",           Type = DrinkType.Beer,     Price = 6.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[0].Id,  Name = "Whiskey Shot",        Type = DrinkType.Shot,     Price = 4.0m,  Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[1].Id,  Name = "Mojito",              Type = DrinkType.Cocktail, Price = 9.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[1].Id,  Name = "Apple Cider",         Type = DrinkType.Cider,    Price = 5.0m,  Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[1].Id,  Name = "Red Wine",            Type = DrinkType.Wine,     Price = 8.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[2].Id,  Name = "Single Malt",         Type = DrinkType.Shot,     Price = 15.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[2].Id,  Name = "Manhattan",           Type = DrinkType.Cocktail, Price = 13.0m, Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[2].Id,  Name = "Stout Beer",          Type = DrinkType.Beer,     Price = 7.0m,  Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[3].Id,  Name = "Tequila Shot",        Type = DrinkType.Shot,     Price = 5.0m,  Flavor = DrinkFlavor.Spicy },
            new Drink { BarId = bars[3].Id,  Name = "Margarita",           Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[3].Id,  Name = "Lager",               Type = DrinkType.Beer,     Price = 4.0m,  Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[4].Id,  Name = "Martini",             Type = DrinkType.Cocktail, Price = 18.0m, Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[4].Id,  Name = "Champagne",           Type = DrinkType.Wine,     Price = 22.0m, Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[4].Id,  Name = "Cognac",              Type = DrinkType.Shot,     Price = 20.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[5].Id,  Name = "Hot Toddy",           Type = DrinkType.Cocktail, Price = 8.0m,  Flavor = DrinkFlavor.Spicy },
            new Drink { BarId = bars[5].Id,  Name = "Mulled Wine",         Type = DrinkType.Wine,     Price = 6.0m,  Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[5].Id,  Name = "Pale Ale",            Type = DrinkType.Beer,     Price = 5.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[6].Id,  Name = "Sazerac",             Type = DrinkType.Cocktail, Price = 14.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[6].Id,  Name = "Negroni",             Type = DrinkType.Cocktail, Price = 11.0m, Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[6].Id,  Name = "Rye Whiskey",         Type = DrinkType.Shot,     Price = 7.0m,  Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[7].Id,  Name = "Vodka Shot",          Type = DrinkType.Shot,     Price = 3.0m,  Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[7].Id,  Name = "Energy Cocktail",     Type = DrinkType.Cocktail, Price = 9.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[7].Id,  Name = "Strong Lager",        Type = DrinkType.Beer,     Price = 4.0m,  Flavor = DrinkFlavor.Spicy },
            new Drink { BarId = bars[8].Id,  Name = "Aperol Spritz",       Type = DrinkType.Cocktail, Price = 14.0m, Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[8].Id,  Name = "Prosecco",            Type = DrinkType.Wine,     Price = 16.0m, Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[8].Id,  Name = "Aged Rum",            Type = DrinkType.Shot,     Price = 17.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[9].Id,  Name = "Gin & Tonic",         Type = DrinkType.Cocktail, Price = 9.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[9].Id,  Name = "Craft Pale Ale",      Type = DrinkType.Beer,     Price = 6.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[9].Id,  Name = "Riesling",            Type = DrinkType.Wine,     Price = 8.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[10].Id, Name = "Rum & Coke",          Type = DrinkType.Cocktail, Price = 7.0m,  Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[10].Id, Name = "Pilsner",             Type = DrinkType.Beer,     Price = 4.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[10].Id, Name = "Tequila Sunrise",     Type = DrinkType.Cocktail, Price = 9.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[11].Id, Name = "Apple Martini",       Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[11].Id, Name = "Spiced Cider",        Type = DrinkType.Cider,    Price = 6.0m,  Flavor = DrinkFlavor.Spicy },
            new Drink { BarId = bars[11].Id, Name = "Cinnamon Whiskey",    Type = DrinkType.Shot,     Price = 5.0m,  Flavor = DrinkFlavor.Spicy },
            new Drink { BarId = bars[12].Id, Name = "Gin Fizz",            Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[12].Id, Name = "Pilsner",             Type = DrinkType.Beer,     Price = 5.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[12].Id, Name = "Rose Wine",           Type = DrinkType.Wine,     Price = 7.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[13].Id, Name = "Jagerbomb",           Type = DrinkType.Shot,     Price = 6.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[13].Id, Name = "Heavy Stout",         Type = DrinkType.Beer,     Price = 5.0m,  Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[13].Id, Name = "Absinth Shot",        Type = DrinkType.Shot,     Price = 8.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[14].Id, Name = "Dom Perignon",        Type = DrinkType.Wine,     Price = 25.0m, Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[14].Id, Name = "Vintage Cognac",      Type = DrinkType.Shot,     Price = 24.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[14].Id, Name = "Vintage Port",        Type = DrinkType.Wine,     Price = 19.0m, Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[15].Id, Name = "Modern Spritz",       Type = DrinkType.Cocktail, Price = 11.0m, Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[15].Id, Name = "Sour Beer",           Type = DrinkType.Beer,     Price = 7.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[15].Id, Name = "Pinot Grigio",        Type = DrinkType.Wine,     Price = 9.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[16].Id, Name = "Hot Buttered Rum",    Type = DrinkType.Cocktail, Price = 8.0m,  Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[16].Id, Name = "Dark Stout",          Type = DrinkType.Beer,     Price = 6.0m,  Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[16].Id, Name = "Bourbon",             Type = DrinkType.Shot,     Price = 9.0m,  Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[17].Id, Name = "Smoked Old Fashioned",Type = DrinkType.Cocktail, Price = 20.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[17].Id, Name = "Vintage Whisky",      Type = DrinkType.Shot,     Price = 28.0m, Flavor = DrinkFlavor.Smoky },
            new Drink { BarId = bars[17].Id, Name = "Premium Champagne",   Type = DrinkType.Wine,     Price = 23.0m, Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[18].Id, Name = "Vodka Red Bull",      Type = DrinkType.Cocktail, Price = 8.0m,  Flavor = DrinkFlavor.Sweet },
            new Drink { BarId = bars[18].Id, Name = "Jager Shot",          Type = DrinkType.Shot,     Price = 5.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[18].Id, Name = "Dry Lager",           Type = DrinkType.Beer,     Price = 4.0m,  Flavor = DrinkFlavor.Herbal },
            new Drink { BarId = bars[19].Id, Name = "Brandy Sour",         Type = DrinkType.Cocktail, Price = 9.0m,  Flavor = DrinkFlavor.Fruity },
            new Drink { BarId = bars[19].Id, Name = "Spiced Mulled Wine",  Type = DrinkType.Wine,     Price = 6.0m,  Flavor = DrinkFlavor.Spicy },
            new Drink { BarId = bars[19].Id, Name = "Dark Rum",            Type = DrinkType.Shot,     Price = 7.0m,  Flavor = DrinkFlavor.Smoky }
        );
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
