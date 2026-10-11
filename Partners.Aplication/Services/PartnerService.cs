
using Partners.Aplication;
using Partners.Application.Mappings;
using Partners.Domain;
using Partners.Domain.Interfaces;

namespace Partners.Application;

public class PartnerService(IPartnerRepository repository) : IPartnerService
{
    private readonly IPartnerRepository _repository = repository;

    public async Task<PartnerDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partner = await _repository.GetByIdAsync(id, cancellationToken);

        return partner is null? null: PartnerMapper.ToDto(partner);
    }

    public async Task<IEnumerable<PartnerDetailsDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var partners = await _repository.GetAllAsync(cancellationToken);

        return [.. partners.AsEnumerable<Partner>().Select(PartnerMapper.ToDto)];
    }

    public async Task<PartnerDetailsDto> CreateAsync(CreatePartnerDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateCreditLimit(dto.CreditLimit);

        if (!Enum.IsDefined(typeof(PartnerType), dto.Type))
        {
            throw new ArgumentException("Tipo de parceiro inválido.", nameof(dto.Type));
        }

        if (await _repository.ExistsByTaxNumberAsync(dto.TaxNumber, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Já existe um parceiro com este número fiscal.");
        }

        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            TaxNumber = dto.TaxNumber,
            Type = dto.Type.ToString(),
            Email = dto.Email,
            CreditLimit = dto.CreditLimit,
            IsActive = true,
            Version = 1
        };

        _repository.Add(partner);

        await _repository.SaveChangesAsync(cancellationToken);

        return PartnerMapper.ToDto(partner);
    }

    public async Task<PartnerDetailsDto?> UpdateAsync(Guid id,UpdatePartnerDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateCreditLimit(dto.CreditLimit);

        if (!Enum.IsDefined(typeof(PartnerType), dto.Type))
        {
            throw new ArgumentException("Tipo de parceiro inválido.", nameof(dto.Type));
        }

        var partner = await _repository.GetByIdAsync(id, cancellationToken);

        if (partner is null)
            return null;

        if (await _repository.ExistsByTaxNumberAsync(dto.TaxNumber, excludedId: id, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Já existe outro parceiro com este número fiscal.");
        }

        partner.Name = dto.Name;
        partner.TaxNumber = dto.TaxNumber;
        partner.Type = dto.Type.ToString(); 
        partner.Email = dto.Email;
        partner.CreditLimit = dto.CreditLimit;
        partner.IsActive = dto.IsActive;
        partner.Version++;
        partner.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return PartnerMapper.ToDto(partner);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partner = await _repository.GetByIdAsync(id, cancellationToken);

        if (partner is null)
            return false;

        _repository.Delete(partner);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateCreditLimit(decimal creditLimit)
    {
        if (creditLimit < 0 || creditLimit > 1_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(creditLimit), "O limite de crédito deve estar entre 0 e 1.000.000.");
        }

        if (decimal.Round(creditLimit, 2) != creditLimit)
        {
            throw new ArgumentException("O limite de crédito não pode ter mais de duas casas decimais.");
        }
    }
}
