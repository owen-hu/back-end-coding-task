using Claims.ApiLayer;

namespace Claims.Core;

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