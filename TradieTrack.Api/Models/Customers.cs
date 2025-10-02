namespace TradieTrack.Api.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Organization? Organization { get; set; }
}
