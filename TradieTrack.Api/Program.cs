using Microsoft.AspNetCore.Mvc;                // for [AsParameters]
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TradieTrack.Api.Contracts;
using TradieTrack.Api.Data;
using TradieTrack.Api.Extensions;
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

// Create org
app.MapPost("/api/organizations", async (AppDbContext db, string name, string plan = "Free", CancellationToken ct = default) =>
{
    if (string.IsNullOrWhiteSpace(name)) return Results.BadRequest("Name is required.");
    var org = new Organization { Name = name.Trim(), Plan = plan };
    db.Organizations.Add(org);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/organizations/{org.Id}", org);
});

app.MapGet("/api/organizations", (AppDbContext db) =>
    db.Organizations
      .OrderBy(o => o.CreatedAt)
      .Select(o => new { o.Id, o.Name, o.Plan, o.CreatedAt })
      .ToList());

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


// / List(paged + search)
app.MapGet("/api/customers", async (
    AppDbContext db,
    [AsParameters] CustomerQuery p,
    CancellationToken ct) =>
{
    var query = db.Customers.AsNoTracking()
        .Where(c => !c.IsDeleted && c.OrganizationId == p.OrganizationId);

    if (!string.IsNullOrWhiteSpace(p.Q))
    {
        var term = p.Q.Trim().ToLower();
        query = query.Where(c =>
            (c.Name != null && c.Name.ToLower().Contains(term)) ||
            (c.Email != null && c.Email.ToLower().Contains(term)) ||
            (c.Phone != null && c.Phone.ToLower().Contains(term)));
    }

    query = query.OrderByDescending(c => c.CreatedAt);

    (List<Customer> items, int total) = await query.ToPagedAsync(p.Page, p.PageSize, ct);
    return Results.Ok(new Paged<Customer>(items, p.Page, p.PageSize, total));
});

// Get by id
app.MapGet("/api/customers/{id:guid}", async (
    AppDbContext db, Guid id, Guid organizationId, CancellationToken ct) =>
{
    var c = await db.Customers.AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId && !x.IsDeleted, ct);

    return c is null ? Results.NotFound() : Results.Ok(c);
});


// Create
app.MapPost("/api/customers", async (
    AppDbContext db, CustomerCreateDto dto, CancellationToken ct) =>
{
    if (dto.OrganizationId == Guid.Empty || string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest("OrganizationId and Name are required.");

    var entity = new Customer
    {
        OrganizationId = dto.OrganizationId,
        Name = dto.Name.Trim(),
        Email = dto.Email?.Trim(),
        Phone = dto.Phone?.Trim(),
        Address = dto.Address?.Trim(),
    };

    db.Customers.Add(entity);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/customers/{entity.Id}", entity);
});

// Update
app.MapPut("/api/customers/{id:guid}", async (
    AppDbContext db, Guid id, Guid organizationId, CustomerUpdateDto dto, CancellationToken ct) =>
{
    var entity = await db.Customers.FirstOrDefaultAsync(
        x => x.Id == id && x.OrganizationId == organizationId && !x.IsDeleted, ct);

    if (entity is null) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(dto.Name)) return Results.BadRequest("Name is required.");

    entity.Name = dto.Name.Trim();
    entity.Email = dto.Email?.Trim();
    entity.Phone = dto.Phone?.Trim();
    entity.Address = dto.Address?.Trim();
    entity.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync(ct);
    return Results.Ok(entity);
});

// Soft delete
app.MapDelete("/api/customers/{id:guid}", async (
    AppDbContext db, Guid id, Guid organizationId, CancellationToken ct) =>
{
    var entity = await db.Customers.FirstOrDefaultAsync(
        x => x.Id == id && x.OrganizationId == organizationId && !x.IsDeleted, ct);

    if (entity is null) return Results.NotFound();

    entity.IsDeleted = true;
    entity.DeletedAt = DateTime.UtcNow;

    await db.SaveChangesAsync(ct);
    return Results.NoContent();
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
