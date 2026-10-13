
using Microsoft.EntityFrameworkCore;
using Partners.Domain;
using Partners.Domain.Interfaces;

namespace Partners.Infra.Data;

public class PartnerRepository(AppDbContext dbContext) : IPartnerRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<(IEnumerable<Partner> Items, int TotalCount)> GetAllAsync(string? search, string? type, bool includeInactive, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Partners.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        if (!string.IsNullOrEmpty(type))
        {
            var normalizedType = type.Trim().ToUpperInvariant();

            query = normalizedType switch
            {
                "CUSTOMER" => query.Where(p => p.Type == "CUSTOMER" || p.Type == "BOTH"),
                "SUPPLIER" => query.Where(p => p.Type == "SUPPLIER" || p.Type == "BOTH"),
                "BOTH" => query.Where(p => p.Type == "BOTH"),
                _ => query.Where(p => p.Type == normalizedType)
            };
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();

            bool isNumeric = long.TryParse(searchTerm, out _);

            query = query.Where(p =>
                (!string.IsNullOrEmpty(p.Name) && EF.Functions.Like(p.Name, $"%{searchTerm}%")) ||
                (isNumeric && !string.IsNullOrEmpty(p.TaxNumber) && p.TaxNumber.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Partner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Partners.FirstOrDefaultAsync(partner => partner.Id == id,cancellationToken);
    }

    public async Task<bool> ExistsByTaxNumberAsync(string taxNumber,Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Partners.Where(partner => partner.TaxNumber == taxNumber);

        if (excludedId.HasValue)
        {
            query = query.Where(partner => partner.Id != excludedId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public void Add(Partner partner)
    {
        _dbContext.Partners.Add(partner);
    }

    public void Update(Partner partner)
    {
        _dbContext.Partners.Update(partner);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
