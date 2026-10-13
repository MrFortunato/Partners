
using Partners.Application.Pagination;
using Partners.Domain;

namespace Partners.Application;

public interface IPartnerService
{
    Task<PartnerDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResult<PartnerDetailsDto>> GetAllAsync(string? search, PartnerType? type, bool includeInactive, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PartnerDetailsDto> CreateAsync(CreatePartnerDto dto, CancellationToken cancellationToken = default);
    Task<PartnerDetailsDto?> UpdateAsync(Guid id, UpdatePartnerDto dto, CancellationToken cancellationToken = default);
}
