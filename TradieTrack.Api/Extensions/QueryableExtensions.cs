using Microsoft.EntityFrameworkCore;

namespace TradieTrack.Api.Extensions;

public static class QueryableExtensions
{
    public static async Task<(List<T> Items, int Total)> ToPagedAsync<T>(
        this IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 200) pageSize = 20;

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}
