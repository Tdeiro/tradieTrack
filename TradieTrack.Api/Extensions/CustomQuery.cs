public record CustomerQuery(Guid OrganizationId, string? Q, int Page = 1, int PageSize = 20);
