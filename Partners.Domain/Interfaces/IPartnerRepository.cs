
using Partners.Domain;

namespace Partners.Domain.Interfaces;

public interface IPartnerRepository
{
    Task<IEnumerable<Partner>> GetAllAsync(CancellationToken cancellationToken = default);
}
