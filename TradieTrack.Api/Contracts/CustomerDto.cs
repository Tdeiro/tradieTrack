namespace TradieTrack.Api.Contracts;

public record CustomerCreateDto(Guid OrganizationId, string Name, string? Email, string? Phone, string? Address);
public record CustomerUpdateDto(string Name, string? Email, string? Phone, string? Address);
public record Paged<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
