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
<<<<<<< HEAD
builder.Services.AddScoped<IBlackjackService, BlackjackService>();
=======
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<ICocktailRecipeService, CocktailRecipeService>();
builder.Services.AddHttpClient<OpenWeatherInterface>();
builder.Services.AddHttpClient<GoogleMapsInterface>();
>>>>>>> 4e2a57b0275e3df79edbdf0ef911869d606f1b66

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
            new() { Name = "Prohibicija",     XCoord = 54.8972, YCoord = 23.8860, Rating = 4.5, Address = "Pelesos g. 5, Kaunas",     OpenTime = new TimeOnly(22, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Indoor },
            new() { Name = "Pabo Latino",     XCoord = 54.9001, YCoord = 23.9100, Rating = 3.8, Address = "Vilniaus g. 24, Kaunas",   OpenTime = new TimeOnly(21, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "Whiskey Den",     XCoord = 54.8985, YCoord = 23.9036, Rating = 4.7, Address = "Laisves al. 10, Kaunas",   OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0),  Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Counter },
            new() { Name = "NeonHouse",       XCoord = 54.9050, YCoord = 23.9200, Rating = 4.2, Address = "Savanoriu pr. 1, Kaunas",  OpenTime = new TimeOnly(20, 0), CloseTime = new TimeOnly(4, 0),  Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Party,  Seating = BarSeating.Indoor },
            new() { Name = "Sky Lounge",      XCoord = 54.8930, YCoord = 23.8950, Rating = 4.9, Address = "Donelaicio g. 60, Kaunas", OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0),  Design = BarDesign.Luxurious,  Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "The Tipsy Owl",   XCoord = 54.9020, YCoord = 23.8800, Rating = 4.3, Address = "Karaliaus Mindaugo pr. 7, Kaunas", OpenTime = new TimeOnly(16, 0), CloseTime = new TimeOnly(0, 0), Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor },
            new() { Name = "Speakeasy 1920",  XCoord = 54.8960, YCoord = 23.9150, Rating = 4.6, Address = "M. Valanciaus g. 3, Kaunas", OpenTime = new TimeOnly(19, 0), CloseTime = new TimeOnly(3, 0), Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Counter },
            new() { Name = "Pulse Club",      XCoord = 54.9100, YCoord = 23.9050, Rating = 3.5, Address = "K. Petrausko g. 15, Kaunas", OpenTime = new TimeOnly(22, 0), CloseTime = new TimeOnly(5, 0), Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Party,  Seating = BarSeating.Indoor },
            new() { Name = "Velvet Room",     XCoord = 54.8940, YCoord = 23.8900, Rating = 4.8, Address = "S. Daukanto g. 12, Kaunas", OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Luxurious,  Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Indoor },
            new() { Name = "Urban Loft",      XCoord = 54.9010, YCoord = 23.8970, Rating = 4.0, Address = "Putvinskio g. 38, Kaunas",  OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0), Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "The Old Anchor",  XCoord = 54.8995, YCoord = 23.9180, Rating = 3.9, Address = "A. Mickeviciaus g. 5, Kaunas", OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Counter },
            new() { Name = "Cinnamon",        XCoord = 54.9070, YCoord = 23.8990, Rating = 4.4, Address = "Kestucio g. 22, Kaunas",   OpenTime = new TimeOnly(16, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor },
            new() { Name = "Lumen",           XCoord = 54.8920, YCoord = 23.9100, Rating = 4.1, Address = "Gedimino g. 4, Kaunas",    OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(0, 0), Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.NoPreference },
            new() { Name = "Metalica",        XCoord = 54.9085, YCoord = 23.9220, Rating = 3.7, Address = "Tunelio g. 9, Kaunas",     OpenTime = new TimeOnly(21, 0), CloseTime = new TimeOnly(5, 0), Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Party,  Seating = BarSeating.Indoor },
            new() { Name = "Pearl Lounge",    XCoord = 54.8960, YCoord = 23.8830, Rating = 5.0, Address = "Rotuses a. 1, Kaunas",     OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Luxurious,  Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.NoPreference },
            new() { Name = "Tabula Rasa",     XCoord = 54.9030, YCoord = 23.9070, Rating = 4.2, Address = "Birutes g. 17, Kaunas",    OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0), Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "Hideaway",        XCoord = 54.8975, YCoord = 23.9220, Rating = 4.5, Address = "Aleksoto g. 6, Kaunas",    OpenTime = new TimeOnly(19, 0), CloseTime = new TimeOnly(3, 0), Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor },
            new() { Name = "Gold & Smoke",    XCoord = 54.9000, YCoord = 23.8920, Rating = 4.7, Address = "Maironio g. 28, Kaunas",   OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0), Design = BarDesign.Luxurious,  Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Counter },
            new() { Name = "The Forge",       XCoord = 54.9055, YCoord = 23.9130, Rating = 3.4, Address = "Linkuvos g. 11, Kaunas",   OpenTime = new TimeOnly(20, 0), CloseTime = new TimeOnly(4, 0), Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Party,  Seating = BarSeating.Outdoor },
            new() { Name = "Antique Cellar",  XCoord = 54.8940, YCoord = 23.9020, Rating = 4.0, Address = "Vasario 16-osios g. 2, Kaunas", OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(0, 0), Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor },
            new() { Name = "Garliava Cellar",    XCoord = 54.7900, YCoord = 23.8800, Rating = 4.0, Address = "Vilniaus g. 30, Garliava",        OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(0, 0),  Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Counter },
            new() { Name = "Karmelava Lounge",   XCoord = 54.9700, YCoord = 24.0500, Rating = 4.2, Address = "Vilniaus g. 5, Karmelava",         OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0),  Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "Babtai Tavern",      XCoord = 55.0800, YCoord = 23.8500, Rating = 3.8, Address = "Pagiriu g. 2, Babtai",             OpenTime = new TimeOnly(16, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Counter },
            new() { Name = "Vilkija Stout",      XCoord = 55.0300, YCoord = 23.6500, Rating = 4.4, Address = "Kauno g. 18, Vilkija",             OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(0, 0),  Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor },
            new() { Name = "Akademija Loft",     XCoord = 54.9900, YCoord = 23.8000, Rating = 3.9, Address = "Studentu g. 11, Akademija",        OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(1, 0),  Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "Riverside Hideaway", XCoord = 54.8200, YCoord = 23.7800, Rating = 4.6, Address = "Nemuno g. 7, Aleksotas",           OpenTime = new TimeOnly(16, 0), CloseTime = new TimeOnly(0, 0),  Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Outdoor },
            new() { Name = "Forest Watch",       XCoord = 55.0500, YCoord = 23.9800, Rating = 3.7, Address = "Misko g. 14, Domeikava",           OpenTime = new TimeOnly(20, 0), CloseTime = new TimeOnly(4, 0),  Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Party,  Seating = BarSeating.Indoor },
            new() { Name = "Lakeside Sip",       XCoord = 54.7900, YCoord = 24.1000, Rating = 4.3, Address = "Ezero g. 3, Praviena",             OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(2, 0),  Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "Hilltop Tap",        XCoord = 54.9500, YCoord = 24.0800, Rating = 4.0, Address = "Kalvos g. 9, Petrasiunai",         OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0),  Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Counter },
            new() { Name = "Pakuonis Inn",       XCoord = 54.7500, YCoord = 24.0500, Rating = 3.5, Address = "Kauno g. 4, Pakuonis",             OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor },
            new() { Name = "Mauruciai Pub",      XCoord = 54.7300, YCoord = 23.8500, Rating = 3.9, Address = "Kauno g. 22, Mauruciai",           OpenTime = new TimeOnly(16, 0), CloseTime = new TimeOnly(0, 0),  Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Counter },
            new() { Name = "Vandziogala Bar",    XCoord = 55.1000, YCoord = 23.9500, Rating = 3.6, Address = "Centras 1, Vandziogala",           OpenTime = new TimeOnly(20, 0), CloseTime = new TimeOnly(4, 0),  Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Party,  Seating = BarSeating.Outdoor },
            new() { Name = "Suburban Saloon",    XCoord = 54.9800, YCoord = 24.0200, Rating = 4.2, Address = "Sodu g. 12, Raudondvaris",         OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0),  Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Indoor },
            new() { Name = "Northern Light",     XCoord = 55.0200, YCoord = 23.8500, Rating = 4.5, Address = "Saules g. 8, Lapes",               OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0),  Design = BarDesign.Luxurious,  Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.NoPreference },
            new() { Name = "Westwood Whisky",    XCoord = 54.8500, YCoord = 23.7500, Rating = 4.7, Address = "Misko al. 5, Kulautuva",           OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(1, 0),  Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Counter },
            new() { Name = "Eastside Loft",      XCoord = 54.9100, YCoord = 24.0800, Rating = 4.1, Address = "Rytu g. 19, Eiguliai",             OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(0, 0),  Design = BarDesign.Modern,     Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "South Cellar",       XCoord = 54.7700, YCoord = 23.8500, Rating = 3.8, Address = "Pietu g. 6, Garliava",             OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(1, 0),  Design = BarDesign.Vintage,    Atmosphere = BarAtmosphere.Lively, Seating = BarSeating.Counter },
            new() { Name = "Forest Trail Inn",   XCoord = 54.7800, YCoord = 24.0000, Rating = 4.0, Address = "Tako g. 4, Saulesetka",            OpenTime = new TimeOnly(19, 0), CloseTime = new TimeOnly(3, 0),  Design = BarDesign.Industrial, Atmosphere = BarAtmosphere.Party,  Seating = BarSeating.Indoor },
            new() { Name = "Cliffside Lounge",   XCoord = 54.9800, YCoord = 23.7800, Rating = 4.6, Address = "Kalno g. 11, Vilijampole",         OpenTime = new TimeOnly(18, 0), CloseTime = new TimeOnly(2, 0),  Design = BarDesign.Luxurious,  Atmosphere = BarAtmosphere.Music,  Seating = BarSeating.Outdoor },
            new() { Name = "Twin Pines Bar",     XCoord = 55.0500, YCoord = 24.0200, Rating = 4.3, Address = "Pusu g. 2, Margininkai",           OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(23, 0), Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor },
            new() { Name = "TEST",               XCoord = 54.9931, YCoord = 23.9036, Rating = 5.0, Address = "Test St. 1, Kaunas",              OpenTime = new TimeOnly(17, 0), CloseTime = new TimeOnly(2, 0),  Design = BarDesign.Cozy,       Atmosphere = BarAtmosphere.Quiet,  Seating = BarSeating.Indoor }
        };
        db.Bars.AddRange(bars);
        db.SaveChanges();

        db.Drinks.AddRange(
            new Drink { BarId = bars[0].Id, Name = "Old Fashioned", Type = DrinkType.Cocktail, Price = 12.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[0].Id, Name = "House IPA", Type = DrinkType.Beer, Price = 6.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[0].Id, Name = "Whiskey Shot", Type = DrinkType.Shot, Price = 4.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[1].Id, Name = "Mojito", Type = DrinkType.Cocktail, Price = 9.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[1].Id, Name = "Apple Cider", Type = DrinkType.Cider, Price = 5.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[1].Id, Name = "Red Wine", Type = DrinkType.Wine, Price = 8.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[2].Id, Name = "Single Malt", Type = DrinkType.Shot, Price = 15.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[2].Id, Name = "Manhattan", Type = DrinkType.Cocktail, Price = 13.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[2].Id, Name = "Stout Beer", Type = DrinkType.Beer, Price = 7.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[3].Id, Name = "Tequila Shot", Type = DrinkType.Shot, Price = 5.0m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[3].Id, Name = "Margarita", Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[3].Id, Name = "Lager", Type = DrinkType.Beer, Price = 4.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[4].Id, Name = "Martini", Type = DrinkType.Cocktail, Price = 18.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[4].Id, Name = "Champagne", Type = DrinkType.Wine, Price = 22.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[4].Id, Name = "Cognac", Type = DrinkType.Shot, Price = 20.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[5].Id, Name = "Hot Toddy", Type = DrinkType.Cocktail, Price = 8.0m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[5].Id, Name = "Mulled Wine", Type = DrinkType.Wine, Price = 6.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[5].Id, Name = "Pale Ale", Type = DrinkType.Beer, Price = 5.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[6].Id, Name = "Sazerac", Type = DrinkType.Cocktail, Price = 14.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[6].Id, Name = "Negroni", Type = DrinkType.Cocktail, Price = 11.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[6].Id, Name = "Rye Whiskey", Type = DrinkType.Shot, Price = 7.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[7].Id, Name = "Vodka Shot", Type = DrinkType.Shot, Price = 3.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[7].Id, Name = "Energy Cocktail", Type = DrinkType.Cocktail, Price = 9.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[7].Id, Name = "Strong Lager", Type = DrinkType.Beer, Price = 4.0m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[8].Id, Name = "Aperol Spritz", Type = DrinkType.Cocktail, Price = 14.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[8].Id, Name = "Prosecco", Type = DrinkType.Wine, Price = 16.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[8].Id, Name = "Aged Rum", Type = DrinkType.Shot, Price = 17.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[9].Id, Name = "Gin & Tonic", Type = DrinkType.Cocktail, Price = 9.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[9].Id, Name = "Craft Pale Ale", Type = DrinkType.Beer, Price = 6.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[9].Id, Name = "Riesling", Type = DrinkType.Wine, Price = 8.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[10].Id, Name = "Rum & Coke", Type = DrinkType.Cocktail, Price = 7.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[10].Id, Name = "Pilsner", Type = DrinkType.Beer, Price = 4.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[10].Id, Name = "Tequila Sunrise", Type = DrinkType.Cocktail, Price = 9.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[11].Id, Name = "Apple Martini", Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[11].Id, Name = "Spiced Cider", Type = DrinkType.Cider, Price = 6.0m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[11].Id, Name = "Cinnamon Whiskey", Type = DrinkType.Shot, Price = 5.0m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[12].Id, Name = "Gin Fizz", Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[12].Id, Name = "Pilsner", Type = DrinkType.Beer, Price = 5.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[12].Id, Name = "Rose Wine", Type = DrinkType.Wine, Price = 7.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[13].Id, Name = "Jagerbomb", Type = DrinkType.Shot, Price = 6.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[13].Id, Name = "Heavy Stout", Type = DrinkType.Beer, Price = 5.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[13].Id, Name = "Absinth Shot", Type = DrinkType.Shot, Price = 8.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[14].Id, Name = "Dom Perignon", Type = DrinkType.Wine, Price = 25.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[14].Id, Name = "Vintage Cognac", Type = DrinkType.Shot, Price = 24.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[14].Id, Name = "Vintage Port", Type = DrinkType.Wine, Price = 19.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[15].Id, Name = "Modern Spritz", Type = DrinkType.Cocktail, Price = 11.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[15].Id, Name = "Sour Beer", Type = DrinkType.Beer, Price = 7.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[15].Id, Name = "Pinot Grigio", Type = DrinkType.Wine, Price = 9.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[16].Id, Name = "Hot Buttered Rum", Type = DrinkType.Cocktail, Price = 8.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[16].Id, Name = "Dark Stout", Type = DrinkType.Beer, Price = 6.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[16].Id, Name = "Bourbon", Type = DrinkType.Shot, Price = 9.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[17].Id, Name = "Smoked Old Fashioned", Type = DrinkType.Cocktail, Price = 20.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[17].Id, Name = "Vintage Whisky", Type = DrinkType.Shot, Price = 28.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[17].Id, Name = "Premium Champagne", Type = DrinkType.Wine, Price = 23.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[18].Id, Name = "Vodka Red Bull", Type = DrinkType.Cocktail, Price = 8.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[18].Id, Name = "Jager Shot", Type = DrinkType.Shot, Price = 5.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[18].Id, Name = "Dry Lager", Type = DrinkType.Beer, Price = 4.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[19].Id, Name = "Brandy Sour", Type = DrinkType.Cocktail, Price = 9.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[19].Id, Name = "Spiced Mulled Wine", Type = DrinkType.Wine, Price = 6.0m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[19].Id, Name = "Dark Rum", Type = DrinkType.Shot, Price = 7.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[20].Id, Name = "Cellar Whiskey", Type = DrinkType.Shot, Price = 8.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[20].Id, Name = "Country Ale", Type = DrinkType.Beer, Price = 4.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[21].Id, Name = "Spritz", Type = DrinkType.Cocktail, Price = 11.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[21].Id, Name = "House Wine", Type = DrinkType.Wine, Price = 7.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[22].Id, Name = "Local Beer", Type = DrinkType.Beer, Price = 3.5m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[22].Id, Name = "Spiced Rum", Type = DrinkType.Shot, Price = 6.0m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[23].Id, Name = "Dark Stout", Type = DrinkType.Beer, Price = 5.5m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[23].Id, Name = "Dry Cider", Type = DrinkType.Cider, Price = 4.5m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[24].Id, Name = "Espresso Martini", Type = DrinkType.Cocktail, Price = 12.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[24].Id, Name = "Pinot Noir", Type = DrinkType.Wine, Price = 9.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[25].Id, Name = "Pear Cider", Type = DrinkType.Cider, Price = 5.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[25].Id, Name = "Wheat Beer", Type = DrinkType.Beer, Price = 5.5m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[26].Id, Name = "Vodka Bomb", Type = DrinkType.Shot, Price = 4.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[26].Id, Name = "Dark Lager", Type = DrinkType.Beer, Price = 4.5m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[27].Id, Name = "Berry Cocktail", Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[27].Id, Name = "Chardonnay", Type = DrinkType.Wine, Price = 8.5m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[28].Id, Name = "IPA", Type = DrinkType.Beer, Price = 5.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[28].Id, Name = "Tequila", Type = DrinkType.Shot, Price = 5.5m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[29].Id, Name = "Mulled Wine", Type = DrinkType.Wine, Price = 6.5m, Flavor = DrinkFlavor.Spicy, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[29].Id, Name = "Bourbon", Type = DrinkType.Shot, Price = 9.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[30].Id, Name = "Apple Cider", Type = DrinkType.Cider, Price = 4.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[30].Id, Name = "Lager", Type = DrinkType.Beer, Price = 3.5m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[31].Id, Name = "Jagermeister", Type = DrinkType.Shot, Price = 6.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[31].Id, Name = "Pilsner", Type = DrinkType.Beer, Price = 3.5m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[32].Id, Name = "Smoke Cocktail", Type = DrinkType.Cocktail, Price = 12.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[32].Id, Name = "Helles Beer", Type = DrinkType.Beer, Price = 5.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[33].Id, Name = "Vintage Champagne", Type = DrinkType.Wine, Price = 24.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[33].Id, Name = "Premium Cognac", Type = DrinkType.Shot, Price = 26.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[34].Id, Name = "Aged Whisky", Type = DrinkType.Shot, Price = 14.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[34].Id, Name = "Porter", Type = DrinkType.Beer, Price = 6.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[35].Id, Name = "Aperol Spritz", Type = DrinkType.Cocktail, Price = 10.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[35].Id, Name = "Pilsner", Type = DrinkType.Beer, Price = 4.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[36].Id, Name = "Brandy", Type = DrinkType.Shot, Price = 7.5m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[36].Id, Name = "Sweet Wine", Type = DrinkType.Wine, Price = 7.5m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[37].Id, Name = "Mezcal Shot", Type = DrinkType.Shot, Price = 6.5m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sour },
            new Drink { BarId = bars[37].Id, Name = "Pale Lager", Type = DrinkType.Beer, Price = 4.5m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[38].Id, Name = "Dry Martini", Type = DrinkType.Cocktail, Price = 15.0m, Flavor = DrinkFlavor.Herbal, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[38].Id, Name = "Vintage Wine", Type = DrinkType.Wine, Price = 20.0m, Flavor = DrinkFlavor.Fruity, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Mixed },
            new Drink { BarId = bars[39].Id, Name = "Pear Cider", Type = DrinkType.Cider, Price = 4.5m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Light, FlavorBalance = DrinkFlavorBalance.Sweet },
            new Drink { BarId = bars[39].Id, Name = "Stout", Type = DrinkType.Beer, Price = 5.0m, Flavor = DrinkFlavor.Smoky, Strength = DrinkStrength.Medium, FlavorBalance = DrinkFlavorBalance.Bitter },
            new Drink { BarId = bars[40].Id, Name = "TEST Sweet Shot", Type = DrinkType.Cocktail, Price = 4.0m, Flavor = DrinkFlavor.Sweet, Strength = DrinkStrength.Strong, FlavorBalance = DrinkFlavorBalance.Sweet }
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

    if (!db.Ingredients.Any())
    {
        db.Ingredients.AddRange(
            new Ingredient { Name = "Vodka",            Category = IngredientCategory.Spirit,   AbvPercentage = 40, Price = 18.0m },
            new Ingredient { Name = "Gin",              Category = IngredientCategory.Spirit,   AbvPercentage = 40, Price = 22.0m },
            new Ingredient { Name = "White Rum",        Category = IngredientCategory.Spirit,   AbvPercentage = 40, Price = 20.0m },
            new Ingredient { Name = "Dark Rum",         Category = IngredientCategory.Spirit,   AbvPercentage = 40, Price = 24.0m },
            new Ingredient { Name = "Tequila Blanco",   Category = IngredientCategory.Spirit,   AbvPercentage = 38, Price = 26.0m },
            new Ingredient { Name = "Bourbon Whiskey",  Category = IngredientCategory.Spirit,   AbvPercentage = 45, Price = 30.0m },
            new Ingredient { Name = "Rye Whiskey",      Category = IngredientCategory.Spirit,   AbvPercentage = 45, Price = 32.0m },
            new Ingredient { Name = "Brandy",           Category = IngredientCategory.Spirit,   AbvPercentage = 40, Price = 28.0m },
            new Ingredient { Name = "Cheap Vodka",      Category = IngredientCategory.Spirit,   AbvPercentage = 37.5, Price = 9.0m },
            new Ingredient { Name = "Triple Sec",       Category = IngredientCategory.Liqueur,  AbvPercentage = 30, Price = 14.0m },
            new Ingredient { Name = "Cointreau",        Category = IngredientCategory.Liqueur,  AbvPercentage = 40, Price = 28.0m },
            new Ingredient { Name = "Campari",          Category = IngredientCategory.Liqueur,  AbvPercentage = 25, Price = 22.0m },
            new Ingredient { Name = "Aperol",           Category = IngredientCategory.Liqueur,  AbvPercentage = 11, Price = 18.0m },
            new Ingredient { Name = "Sweet Vermouth",   Category = IngredientCategory.Wine,     AbvPercentage = 16, Price = 12.0m },
            new Ingredient { Name = "Dry Vermouth",     Category = IngredientCategory.Wine,     AbvPercentage = 18, Price = 12.0m },
            new Ingredient { Name = "Prosecco",         Category = IngredientCategory.Wine,     AbvPercentage = 11, Price = 14.0m },
            new Ingredient { Name = "Lime Juice",       Category = IngredientCategory.Juice,    AbvPercentage = 0,  Price = 3.5m },
            new Ingredient { Name = "Lemon Juice",      Category = IngredientCategory.Juice,    AbvPercentage = 0,  Price = 3.5m },
            new Ingredient { Name = "Orange Juice",     Category = IngredientCategory.Juice,    AbvPercentage = 0,  Price = 3.0m },
            new Ingredient { Name = "Cranberry Juice",  Category = IngredientCategory.Juice,    AbvPercentage = 0,  Price = 3.0m },
            new Ingredient { Name = "Simple Syrup",     Category = IngredientCategory.Syrup,    AbvPercentage = 0,  Price = 2.5m },
            new Ingredient { Name = "Grenadine",        Category = IngredientCategory.Syrup,    AbvPercentage = 0,  Price = 4.0m },
            new Ingredient { Name = "Tonic Water",      Category = IngredientCategory.Mixer,    AbvPercentage = 0,  Price = 1.5m },
            new Ingredient { Name = "Soda Water",       Category = IngredientCategory.Mixer,    AbvPercentage = 0,  Price = 1.0m },
            new Ingredient { Name = "Ginger Beer",      Category = IngredientCategory.Mixer,    AbvPercentage = 0,  Price = 2.0m },
            new Ingredient { Name = "Cola",             Category = IngredientCategory.Mixer,    AbvPercentage = 0,  Price = 1.5m },
            new Ingredient { Name = "Angostura Bitters",Category = IngredientCategory.Bitter,   AbvPercentage = 44.7, Price = 16.0m },
            new Ingredient { Name = "Orange Bitters",   Category = IngredientCategory.Bitter,   AbvPercentage = 28, Price = 14.0m },
            new Ingredient { Name = "Mint Leaves",      Category = IngredientCategory.Garnish,  AbvPercentage = 0,  Price = 1.0m },
            new Ingredient { Name = "Lime Wedge",       Category = IngredientCategory.Garnish,  AbvPercentage = 0,  Price = 0.5m }
        );
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
