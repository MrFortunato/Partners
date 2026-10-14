using AutoMapper;
using Partners.Application.Exceptions;
using Partners.Application.Pagination;
using Partners.Domain;
using Partners.Domain.Interfaces;

namespace Partners.Application;

public class PartnerService(IPartnerRepository repository, IMapper mapper) : IPartnerService
{
    private readonly IPartnerRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<PartnerDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partner = await _repository.GetByIdAsync(id, cancellationToken);

        return partner is null ? null : _mapper.Map<PartnerDetailsDto>(partner);
    }

    public async Task<PaginatedResult<PartnerDetailsDto>> GetAllAsync(string? search, PartnerType? type, bool includeInactive, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        string? typeString = type?.ToString();

        var (partners, totalCount) = await _repository.GetAllAsync(search, typeString, includeInactive, page, pageSize, cancellationToken);

        var items = _mapper.Map<List<PartnerDetailsDto>>(partners);

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

        var cleanedTaxNumber = NormalizeTaxNumber(dto.TaxNumber);

        if (await _repository.ExistsByTaxNumberAsync(cleanedTaxNumber, cancellationToken: cancellationToken))
        {
            throw new ConflictException("Já existe outra Entidade registado com este número fiscal.");
        }

        var partner = _mapper.Map<Partner>(dto);
        partner.Id = Guid.NewGuid();
        partner.TaxNumber = cleanedTaxNumber;
        partner.Name = partner.Name.Trim();
        partner.Version = 1;
        partner.CreatedAt = DateTime.UtcNow;

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

        var cleanedTaxNumber = NormalizeTaxNumber(dto.TaxNumber);

        var existingPartnerWithTax = await _repository.ExistsByTaxNumberAsync(cleanedTaxNumber, id, cancellationToken);
        if (existingPartnerWithTax)
        {
            throw new ConflictException("Já existe outra Entidade registado com este número fiscal.");
        }

        _mapper.Map(dto, partner);
        partner.TaxNumber = cleanedTaxNumber;
        partner.Name = partner.Name?.Trim() ?? string.Empty;
        partner.UpdatedAt = DateTime.UtcNow;
        partner.Version += 1;

        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PartnerDetailsDto>(partner);
    }



    private static void ValidateCreditLimit(decimal creditLimit)
    {
        if (creditLimit < 0 || creditLimit > 1_000_000)
        {
            throw new ArgumentException("O limite de crédito deve estar entre 0 e 1.000.000.", nameof(creditLimit));
        }

        if (decimal.Round(creditLimit, 2) != creditLimit)
        {
            throw new ArgumentException("O limite de crédito não pode ter mais de duas casas decimais.");
        }
    }

    /// <summary>
    /// Normaliza o número fiscal: rejeita caracteres inválidos (letras, símbolos),
    /// descarta apenas separadores de formatação (., espaço, -) e exige 9 dígitos.
    /// </summary>
    private static string NormalizeTaxNumber(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new ArgumentException("O número fiscal é obrigatório.");
        }

        var trimmed = raw.Trim();

        // Rejeita qualquer caractere que não seja dígito nem separador de formatação.
        if (trimmed.Any(c => !char.IsDigit(c) && c != '.' && c != ' ' && c != '-'))
        {
            throw new ArgumentException("O número fiscal contém caracteres inválidos. Apenas dígitos, pontos, espaços e hífens são permitidos.");
        }

        var digits = new string(trimmed.Where(char.IsDigit).ToArray());

        if (digits.Length != 9)
        {
            throw new ArgumentException("O número fiscal deve conter exatamente 9 dígitos numéricos.");
        }

        return digits;
    }
}
