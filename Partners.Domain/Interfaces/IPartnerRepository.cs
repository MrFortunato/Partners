namespace Partners.Domain.Interfaces;

public interface IPartnerRepository
{
    Task<(IEnumerable<Partner> Items, int TotalCount)> GetAllAsync(string? search, string? type, bool includeInactive, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<Partner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByTaxNumberAsync(string taxNumber, Guid? excludedId = null, CancellationToken cancellationToken = default);

    void Add(Partner partner);

    void Update(Partner partner);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
