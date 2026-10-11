using Microsoft.AspNetCore.Mvc;
using Partners.Aplication;
using Partners.Infra.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Partners.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerService _partnerService;

        public PartnersController(IPartnerService partnerService)
        {
            _partnerService = partnerService; 
        }
        // GET: api/<PartnersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PartnersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PartnersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PartnersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PartnersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
