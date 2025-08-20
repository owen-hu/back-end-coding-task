using Claims.ApiLayer;
using Claims.DataLayer.Claims;

namespace Claims.Core;

public interface IPremiumCalculator
{
    /// <summary>
    /// Derive based on start and end dates + cover type
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="coverType"></param>
    /// <returns></returns>
    decimal Calculate(DateOnly startDate, DateOnly endDate, CoverType coverType);
}

public static class PremiumCalculatorExtensions
{
    /// <summary>
    /// Derive based on <see cref="CoverDto"/>
    /// </summary>
    /// <param name="calculator"></param>
    /// <param name="coverDto"></param>
    /// <returns></returns>
    public static decimal Calculate(this IPremiumCalculator calculator, CoverDto coverDto)
    {
        return calculator.Calculate(coverDto.StartDate, coverDto.EndDate, coverDto.CoverType);
    }
}