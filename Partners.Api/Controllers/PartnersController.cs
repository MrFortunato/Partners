using Microsoft.AspNetCore.Mvc;
using Partners.Aplication;
using Partners.Application;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Partners.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController(IPartnerService partnerService) : ControllerBase
    {
        private readonly IPartnerService _partnerService = partnerService;

        // GET: api/partners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartnerDetailsDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var partners = await _partnerService.GetAllAsync(cancellationToken);

            return Ok(partners);
        }

        // GET: api/partners/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PartnerDetailsDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var partner = await _partnerService.GetByIdAsync(id, cancellationToken);

            if (partner is null)
            {
                throw new KeyNotFoundException("Parceiro não encontrado.");
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
        

        // PUT api/<PartnersController>/5
        // PUT: api/partners/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdatePartnerDto dto, CancellationToken cancellationToken = default)
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

            return NoContent(); 
        }

    }
}
