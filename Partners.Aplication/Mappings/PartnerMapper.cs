using Partners.Domain;

namespace Partners.Application.Mappings;

public static class PartnerMapper
{
    public static PartnerDetailsDto ToDto(Partner partner)
    {
		_ = Enum.TryParse<PartnerType>(partner.Type, out var parsedType);

        return new PartnerDetailsDto
        {
            Id = partner.Id,
            Name = partner.Name,
            TaxNumber = partner.TaxNumber ?? string.Empty,
            Type = parsedType,
            Email = partner.Email,
            CreditLimit = partner.CreditLimit,
            IsActive = partner.IsActive,
            Version = partner.Version,
            CreatedAt = partner.CreatedAt
        };
    }
}
