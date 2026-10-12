
using AutoMapper;
using Partners.Application.Exceptions;
using Partners.Application.Pagination;
using Partners.Domain;
using Partners.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Partners.Application;

public class PartnerService(IPartnerRepository repository, IMapper mapper) : IPartnerService
{
    private readonly IPartnerRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<PartnerDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partner = await _repository.GetByIdAsync(id, cancellationToken);

        return partner is null ? null: _mapper.Map<PartnerDetailsDto>(partner); ;
    }

    public async Task<PaginatedResult<PartnerDetailsDto>> GetAllAsync(string? search, PartnerType? type, bool includeInactive, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        string? typeString = type?.ToString();

        var (partners, totalCount) = await _repository.GetAllAsync(search, typeString, includeInactive, page, pageSize, cancellationToken);

        var items = _mapper.Map<List<PartnerDetailsDto>>(partners); ;

        return new PaginatedResult<PartnerDetailsDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PartnerDetailsDto> CreateAsync(CreatePartnerDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateCreditLimit(dto.CreditLimit);

        if (!Enum.IsDefined(typeof(PartnerType), dto.Type))
        {
            throw new ArgumentException("Tipo de Entidade inválido.", nameof(dto.Type));
        }

        var cleanedTaxNumber = Regex.Replace(dto.TaxNumber ?? string.Empty, @"\D", "");

        if (cleanedTaxNumber.Length != 9)
        {
            throw new ArgumentException("O número fiscal deve conter exatamente 9 dígitos numéricos.");
        }

        if (await _repository.ExistsByTaxNumberAsync(cleanedTaxNumber, cancellationToken: cancellationToken))
        {
            throw new ConflictException("Já existe outra Entidade registado com este número fiscal.");
        }

        var partner = _mapper.Map<Partner>(dto);
        partner.Id = Guid.NewGuid();

        _repository.Add(partner);

        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PartnerDetailsDto>(partner);
    }

    public async Task<PartnerDetailsDto?> UpdateAsync(Guid id,UpdatePartnerDto dto, CancellationToken cancellationToken = default)
    {
        var partner = await _repository.GetByIdAsync(id, cancellationToken);
        if (partner is null)
        {
            throw new KeyNotFoundException("Entidade não encontrado.");
        }

        if (partner.Version != dto.Version)
        {
            throw new ConflictException("O registo foi modificado por outro utilizador. Por favor, atualize os dados.");
        }

        var cleanedTaxNumber = Regex.Replace(dto.TaxNumber ?? string.Empty, @"\D", "");
        if (cleanedTaxNumber.Length != 9)
        {
            throw new ArgumentException("O número fiscal deve conter exatamente 9 dígitos numéricos.");
        }

        var existingPartnerWithTax = await _repository.ExistsByTaxNumberAsync(cleanedTaxNumber, id, cancellationToken);
        if (existingPartnerWithTax)
        {
            throw new ConflictException("Já existe outra Entidade registado com este número fiscal.");
        }

        partner.Name = dto.Name.Trim();
        partner.TaxNumber = cleanedTaxNumber;
        partner.Type = dto.Type.ToString();
        partner.Email = dto.Email;
        partner.CreditLimit = dto.CreditLimit;
        partner.IsActive = dto.IsActive; 
        partner.UpdatedAt = DateTime.UtcNow;
        partner.Version += 1;

        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PartnerDetailsDto>(partner);
    }



    private static void ValidateCreditLimit(decimal creditLimit)
    {
        if (creditLimit < 0 || creditLimit > 1_000_000)
        {
            throw new ArgumentException(nameof(creditLimit), "O limite de crédito deve estar entre 0 e 1.000.000.");
        }

        if (decimal.Round(creditLimit, 2) != creditLimit)
        {
            throw new ArgumentException("O limite de crédito não pode ter mais de duas casas decimais.");
        }
    }

  
}
