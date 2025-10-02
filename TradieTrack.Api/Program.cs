using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TradieTrack.Api.Data;
using Microsoft.AspNetCore.Components.Forms;
using TradieTrack.Api.Models;
using TradieTrack.Api.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TradieTrack API", Version = "v1" });
});

var allowedOrigins = new[] { "http://localhost:5173" };
builder.Services.AddCors(o => o.AddPolicy("localdev",
    p => p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod())
);

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(cs));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var shouldSeed =
        env.IsDevelopment() &&
        (Environment.GetEnvironmentVariable("SEED")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true
         || args.Contains("--seed"));

    if (shouldSeed)
        await JsonSeed.RunAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TradieTrack API v1");
        c.RoutePrefix = "swagger"; // UI at /swagger
    });
}

app.UseCors("localdev");


app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

// Users
app.MapGet("api/users", async (AppDbContext db) =>
    await db.Users.AsNoTracking().Take(50).ToListAsync());

app.MapPost("api/users", async (AppDbContext db, User input) =>
{
    if (input.OrganizationId == Guid.Empty || string.IsNullOrWhiteSpace(input.Email))
        return Results.BadRequest("OrganizationId and Email are required.");

    db.Users.Add(input);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{input.Id}", input);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
