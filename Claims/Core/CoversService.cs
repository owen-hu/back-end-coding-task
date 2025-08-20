using Claims.ApiLayer;
using Claims.DataLayer.Claims;
using Microsoft.EntityFrameworkCore;

namespace Claims.Core;

public class CoversService
{
    private readonly IRepository<Cover> _coversRepo;
    private readonly IAuditor _auditor;
    private readonly IValidator<CoverDto> _coverValidator;
    public IPremiumCalculator PremiumCalculator {get; init;}
    
    public CoversService(IRepository<Cover> coversRepo, IAuditor auditor, IPremiumCalculator premiumCalculator, IValidator<CoverDto> coverValidator)
    {
        _coversRepo = coversRepo;
        _auditor = auditor;
        PremiumCalculator = premiumCalculator;
        _coverValidator = coverValidator;
    }
    
    public async Task<IEnumerable<CoverDto>> GetAsync()
    {
        var covers = await _coversRepo.GetItemsAsync();
        return covers.Select(CreateCoverDto);
    }

    public async Task<CoverDto?> GetForIdAsync(string id)
    {
        var cover = await _coversRepo.GetItemAsync(id);
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
        await _coverValidator.ValidateAsync(coverDto);
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
        
        await _coversRepo.AddItemAsync(cover);
        return cover;
    }

    public async Task DeleteAndAuditAsync(string id)
    {
        _auditor.AuditCover(id, "DELETE");
        var cover = await _coversRepo.GetItemAsync(id);
        if (cover is not null)
        {
            await _coversRepo.DeleteItemAsync(cover);
        }
    }
}