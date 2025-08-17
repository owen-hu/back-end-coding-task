using Claims.ApiLayer;
using Claims.Core;
using Microsoft.AspNetCore.Mvc;


namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly ClaimsService _claimsService;

        public ClaimsController(ILogger<ClaimsController> logger, ClaimsService claimsService)
        {
            _logger = logger;
            _claimsService = claimsService;
        }

        [HttpGet]
        public async Task<IEnumerable<ClaimDto>> GetAsync()
        {
            return await _claimsService.GetAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(ClaimDto claim)
        {
            claim = await _claimsService.CreateAndAuditAsync(claim);
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _claimsService.DeleteAndAuditAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(string id)
        {
            return Ok(await _claimsService.GetForIdAsync(id));
        }
    }
}
