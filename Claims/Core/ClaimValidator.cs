using Claims.ApiLayer;
using Claims.DataLayer.Claims;

namespace Claims.Core;

/// <summary>
/// Naive implementation.
/// </summary>
public class ClaimValidator : IValidator<ClaimDto>
{
    const decimal MaxDamageCost = 100000;

    
    private readonly IRepository<Cover> _coverRepo;
    public ClaimValidator(IRepository<Cover> coverRepo)
    {
        _coverRepo = coverRepo;        
    }
    
    public async Task ValidateAsync(ClaimDto item)
    {
        var cover = await _coverRepo.GetItemAsync(item.CoverId);

        if (cover == null)
        {
            throw new ValidationException("Cover not found for claim");
        }
        
        if (item.DamageCost > MaxDamageCost)
        {
            throw new ValidationException($"DamageCost cannot exceed {MaxDamageCost:n0}");
        }
        
        if (item.CreatedDate.DayNumber < cover.StartDate.DayNumber || item.CreatedDate.DayNumber > cover.EndDate.DayNumber )
        {
            throw new ValidationException("Created date must be within the period of the related Cover");
        }
    }

}