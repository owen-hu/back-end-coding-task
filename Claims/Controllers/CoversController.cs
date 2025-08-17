// using Claims.DataLayer.Claims;

using Claims.ApiLayer;
using Claims.Core;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly CoversService  _coversService;
    private readonly ILogger<CoversController> _logger;

    public CoversController(CoversService coversService,ILogger<CoversController> logger)
    {
        _coversService = coversService;
        _logger = logger;
    }

    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var premium = _coversService.PremiumCalculator.Calculate(startDate, endDate, coverType);
        _logger.LogInformation("Premium calculated for {StartDate} to {DateOnly} with CoverType: {CoverType}: {Premium}", 
            startDate, endDate, coverType, premium);
        return Ok(premium);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
    {
        var results = await _coversService.GetAsync();
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(string id)
    {
        var coverDto = await _coversService.GetForIdAsync(id);
        if (coverDto == null)
        {
            return NotFound();
        }
        return Ok(coverDto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CoverDto coverDto)
    {
        coverDto = await _coversService.CreateAndAuditAsync(coverDto);
        return Ok(coverDto);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _coversService.DeleteAndAuditAsync(id);
    }
}
