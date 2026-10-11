
using Microsoft.EntityFrameworkCore;
using Partners.Domain;
using Partners.Domain.Interfaces;

namespace Partners.Infra.Data;

public class PartnerRepository : IPartnerRepository
{
    private readonly AppDbContext _dbContext;

    public PartnerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Partner>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Partners
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Partner?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Partners
            .FirstOrDefaultAsync(
                partner => partner.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByTaxNumberAsync(
        string taxNumber,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Partners
            .Where(partner => partner.TaxNumber == taxNumber);

        if (excludedId.HasValue)
        {
            query = query.Where(
                partner => partner.Id != excludedId.Value);
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

    public void Delete(Partner partner)
    {
        _dbContext.Partners.Remove(partner);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
