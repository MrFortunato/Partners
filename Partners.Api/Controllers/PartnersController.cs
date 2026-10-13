using Microsoft.AspNetCore.Mvc;
using Partners.Application;
using Partners.Application.Pagination;
using Partners.Domain;

namespace Partners.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController(IPartnerService partnerService) : ControllerBase
    {
        private readonly IPartnerService _partnerService = partnerService;

        // GET: api/partners
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PartnerDetailsDto>>> GetPartners(
             [FromQuery] string? search,
             [FromQuery] PartnerType? type,
             [FromQuery] bool includeInactive = false, 
             [FromQuery] int page = 1,                
             [FromQuery] int pageSize = 20,        
             CancellationToken cancellationToken = default)
        {
          
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100; 

            var result = await _partnerService.GetAllAsync(search, type, includeInactive, page, pageSize, cancellationToken);

            return Ok(result);
        }

        // GET: api/partners/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PartnerDetailsDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var partner = await _partnerService.GetByIdAsync(id, cancellationToken);


            if (partner is null)
            {
                return NotFound(new { message = "Entidade não encontrado." });
            }

            return Ok(partner);
        }

        // POST: api/partners
        [HttpPost]
        public async Task<ActionResult<PartnerDetailsDto>> Create([FromBody] CreatePartnerDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPartner = await _partnerService.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = createdPartner.Id }, createdPartner);
        }

        // PUT: api/partners/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PartnerDetailsDto>> Update(Guid id, [FromBody] UpdatePartnerDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPartner = await _partnerService.UpdateAsync(id, dto, cancellationToken);

            if (updatedPartner is null)
            {
                return NotFound();
            }

            return Ok(updatedPartner);
        }
    }
}
