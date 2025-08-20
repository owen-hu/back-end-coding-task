using System.Globalization;
using Claims.ApiLayer;
using Claims.DataLayer.Claims;

namespace Claims.Core;

public interface IValidator<T>
{
    /// <summary>
    /// Error on validation failure
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException">If validation fails</exception>
    Task ValidateAsync(T item);
}

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


public class CoverValidator : IValidator<CoverDto>
{
    
    /// <summary>
    /// Seam for mocking in Unit tests.
    /// </summary>
    protected virtual DateTime Today => DateTime.UtcNow.Date;
    
    public async Task ValidateAsync(CoverDto item)
    {
        if (item.StartDate.DayNumber < DateOnly.FromDateTime(Today).DayNumber)
        {
            throw new ValidationException("StartDate cannot be in the past");
        }

        if (item.StartDate.ToDateTime(TimeOnly.MinValue).AddYears(1) < item.EndDate.ToDateTime(TimeOnly.MinValue))
        {
            throw new ValidationException("total insurance period cannot exceed 1 year");
        }
    }
}