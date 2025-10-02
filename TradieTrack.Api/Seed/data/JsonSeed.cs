using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TradieTrack.Api.Data;
using TradieTrack.Api.Models;

namespace TradieTrack.Api.Seed;

public static class JsonSeed
{
    static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public static async Task RunAsync(AppDbContext db, CancellationToken ct = default)
    {
        // Organizations
        var orgs = Read<List<Organization>>("Seed/data/organizations.json");
        foreach (var org in orgs)
        {
            var exists = await db.Organizations.AnyAsync(o => o.Id == org.Id, ct);
            if (!exists) db.Organizations.Add(org);
        }

        // Users
        var users = Read<List<User>>("Seed/data/users.json");
        foreach (var u in users)
        {
            var exists = await db.Users.AnyAsync(x => x.Id == u.Id || x.Email == u.Email, ct);
            if (!exists) db.Users.Add(u);
        }

        await db.SaveChangesAsync(ct);
    }

    private static T Read<T>(string relativePath)
    {
        var full = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (!File.Exists(full))
            full = Path.Combine(Directory.GetCurrentDirectory(), relativePath); // fallback for ef tooling

        var json = File.ReadAllText(full);
        return JsonSerializer.Deserialize<T>(json, _json)!;
    }
}
