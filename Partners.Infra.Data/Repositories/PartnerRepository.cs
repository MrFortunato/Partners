
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

    public async Task<IEnumerable<Partner>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Partners
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
