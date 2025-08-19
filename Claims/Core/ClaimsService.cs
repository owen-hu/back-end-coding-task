using Claims.ApiLayer;
using Claims.DataLayer.Claims;

namespace Claims.Core;

public class ClaimsService
{
    
    private readonly IRepository<Claim> _claimsRepo;
    private readonly IAuditor _auditor;
    private readonly IValidator<ClaimDto> _validator;
    
    public ClaimsService(IRepository<Claim> claimsRepo, IAuditor auditor,
        IValidator<ClaimDto> validator)
    {
        _claimsRepo = claimsRepo;
        _auditor = auditor;
        _validator = validator;
    }
    public async Task<IEnumerable<ClaimDto>> GetAsync()
    {
        return (await _claimsRepo.GetItemsAsync())
            .Select(CreateClaimDto);
    }

    public async Task<ClaimDto?> GetForIdAsync(string id)
    {
        var claim = await _claimsRepo.GetItemAsync(id);
        if (claim == null)
            return null;
        
        return CreateClaimDto(claim);
    }
    
    /// <summary>
    /// Only creates and Audits if it's valid
    /// </summary>
    /// <param name="claimDto"></param>
    /// <returns></returns>
    public async Task<ClaimDto> CreateAndAuditAsync(ClaimDto claimDto)
    {
        await _validator.ValidateAsync(claimDto); //Will throw an exception if it's not valid.
        var claim = new Claim()
        {
            Id = Guid.NewGuid().ToString(),
            Name = claimDto.Name,
            CoverId = claimDto.CoverId,
            DamageCost = claimDto.DamageCost,
            Created = claimDto.CreatedDate,
            Type = claimDto.ClaimType,
        };
        await _claimsRepo.AddItemAsync(claim);
        _auditor.AuditClaim(claim.Id, "POST");
        return CreateClaimDto(claim);
    }

    public async Task DeleteAndAuditAsync(string id)
    {
        _auditor.AuditClaim(id, "DELETE");
        var claim = await _claimsRepo.GetItemAsync(id);
        if (claim != null)
        {
            await _claimsRepo.DeleteItemAsync(claim);
        }
    }

    private ClaimDto CreateClaimDto(Claim claim)
    {
        return new ClaimDto
        {
            Id = claim.Id,
            Name = claim.Name,
            CreatedDate = claim.Created,
            CoverId = claim.CoverId,
            DamageCost = claim.DamageCost,
            ClaimType = claim.Type,
        };
    }
}