using Claims.ApiLayer;
using Claims.DataLayer.Claims;
using Microsoft.EntityFrameworkCore;


namespace Claims.Core;

public class ClaimsService
{
    
}

public class CoversService
{
    
    private readonly ClaimsContext _claimsContext;
    private readonly Auditor _auditor;
    public IPremiumCalculator PremiumCalculator {get; init;}
    
    public CoversService(ClaimsContext claimsContext, Auditor auditor, IPremiumCalculator premiumCalculator)
    {
        _claimsContext = claimsContext;
        _auditor = auditor;
        PremiumCalculator = premiumCalculator;
    }
    
    public async Task<IEnumerable<CoverDto>> GetAsync()
    {
        var covers = await _claimsContext.Covers.ToListAsync();
        return covers.Select(CreateCoverDto);
    }

    public async Task<CoverDto?> GetForIdAsync(string id)
    {
        var cover = await _claimsContext.Covers.FirstOrDefaultAsync(x => x.Id == id);
        if (cover == null) return null;
        return CreateCoverDto(cover);
    }

    private static CoverDto CreateCoverDto(Cover cover)
    {
        return new CoverDto()
        {
            Id = cover.Id,
            CoverType = cover.Type,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Premium = cover.Premium,
        };
    }

    public async Task<CoverDto> CreateAndAuditAsync(CoverDto coverDto)
    {
        var premium = PremiumCalculator.Calculate(coverDto);
        var cover = await CreateCoverAsync(coverDto, premium);
        
        _auditor.AuditCover(cover.Id, "POST");
        
        return CreateCoverDto(cover); ;
    }

    private async Task<Cover> CreateCoverAsync(CoverDto coverDto, decimal premium)
    {
        var cover = new Cover()
        {
            Id = Guid.NewGuid().ToString(),
            Premium = premium,
            StartDate = coverDto.StartDate,
            EndDate = coverDto.EndDate,
            Type = coverDto.CoverType,
        };
        
        _claimsContext.Covers.Add(cover);
        await _claimsContext.SaveChangesAsync();
        return cover;
    }

    public async Task DeleteAndAuditAsync(string id)
    {
        _auditor.AuditCover(id, "DELETE");
        var cover = await _claimsContext.Covers.Where(cover => cover.Id == id).SingleOrDefaultAsync();
        if (cover is not null)
        {
            _claimsContext.Covers.Remove(cover);
            await _claimsContext.SaveChangesAsync();
        }
    }
}