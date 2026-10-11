using Partners.Domain;

namespace Partners.Application.Mappings;

public static class PartnerMapper
{
    public static PartnerDetailsDto ToDto(Partner partner)
    {
        Enum.TryParse<PartnerType>(partner.Type, out var parsedType);

        return new PartnerDetailsDto
        {
            Id = partner.Id,
            Name = partner.Name,
            TaxNumber = partner.TaxNumber,
            Type = parsedType,
            Email = partner.Email,
            CreditLimit = partner.CreditLimit,
            IsActive = partner.IsActive,
            Version = partner.Version,
            CreatedAt = partner.CreatedAt,

        };
    }

    public static bool TryParseType(
        string type,
        out PartnerType partnerType)
    {
        return Enum.TryParse(type, ignoreCase: true, out partnerType)
            && Enum.IsDefined(typeof(PartnerType), partnerType);
    }
}
