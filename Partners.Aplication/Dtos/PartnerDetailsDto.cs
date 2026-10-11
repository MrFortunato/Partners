
using Partners.Domain;

namespace Partners.Application;

public class PartnerDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string TaxNumber { get; set; } = string.Empty;

    public PartnerType Type { get; set; }

    public string? Email { get; set; }

    public decimal CreditLimit { get; set; }

    public bool IsActive { get; set; }

    public int Version { get; set; }    

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
