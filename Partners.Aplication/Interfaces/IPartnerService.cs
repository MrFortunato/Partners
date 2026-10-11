
using Partners.Application;

namespace Partners.Aplication;

public interface IPartnerService
{
    Task<PartnerDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<PartnerDetailsDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PartnerDetailsDto> CreateAsync(CreatePartnerDto dto, CancellationToken cancellationToken = default);

    Task<PartnerDetailsDto?> UpdateAsync(Guid id,UpdatePartnerDto dto, CancellationToken cancellationToken = default);

}
