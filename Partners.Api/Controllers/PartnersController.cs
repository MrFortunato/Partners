using Microsoft.AspNetCore.Mvc;
using Partners.Aplication;
using Partners.Application;
using System.Text.RegularExpressions;

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
                return NotFound(); 
            }

            return Ok(partner); 
        }

        // POST: api/partners
        [HttpPost]
        public async Task<ActionResult<PartnerDetailsDto>> Create([FromBody] CreatePartnerDto dto, CancellationToken cancellationToken = default)
        {
            //var cleanedTaxNumber = Regex.Replace(dto.TaxNumber ?? string.Empty, @"\D", "");
            //if (cleanedTaxNumber.Length != 9)
            //{
            //    throw new ArgumentException("O TaxNumber deve conter exatamente 9 dígitos numéricos.");
            //}

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var createdPartner = await _partnerService.CreateAsync(dto, cancellationToken);

            //return CreatedAtAction(nameof(GetById), new { id = createdPartner.Id }, createdPartner);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Se o serviço lançar um "throw new ArgumentException("Já existe...")", 
            // o middleware global interceta-o e devolve automaticamente um HTTP 400 com a mensagem amigável!
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

        // DELETE: api/partners/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var deleted = await _partnerService.DeleteAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound(); 
            }

            return NoContent(); 
        }
    }
}
