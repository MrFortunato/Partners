using Partners.Domain;

namespace Partners.Aplication.Tests.TestData;

internal static class PartnerTestData
{
    public static readonly Guid ExistingId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid NonExistentId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public const string ValidName = "Acme Lda";
    public const string ValidTaxNumber = "999999990";
    public const string DuplicateTaxNumber = "999999999";
    public const string Email = "contact@acme.ao";
    public const decimal ValidCreditLimit = 5000m;

    public static Partner BuildPartner(
        Guid? id = null,
        string name = ValidName,
        string taxNumber = ValidTaxNumber,
        PartnerType type = PartnerType.CUSTOMER,
        string? email = Email,
        decimal creditLimit = ValidCreditLimit,
        bool isActive = true,
        int version = 1)
    {
        return new Partner
        {
            Id = id ?? ExistingId,
            Name = name,
            TaxNumber = taxNumber,
            Type = type.ToString(),
            Email = email,
            CreditLimit = creditLimit,
            IsActive = isActive,
            Version = version,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
    }
}
