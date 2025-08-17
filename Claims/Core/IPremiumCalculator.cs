using Claims.ApiLayer;
using Claims.DataLayer.Claims;

namespace Claims.Core;

public interface IPremiumCalculator
{
    decimal Calculate(DateOnly startDate, DateOnly endDate, CoverType coverType);
}

public static class PremiumCalculatorExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="calculator"></param>
    /// <param name="coverDto"></param>
    /// <returns></returns>
    public static decimal Calculate(this IPremiumCalculator calculator, CoverDto coverDto)
    {
        return calculator.Calculate(coverDto.StartDate, coverDto.EndDate, coverDto.CoverType);
    }
}

/// <summary>
/// Logic seems to be based on Tranches.
/// </summary>
public class TranchedPremiumCalculator : IPremiumCalculator
{
    private class PremiumTranche
    {
        public decimal Multiplier;
        public int FromDay;
        public int ToDay;
        public CoverType? CoverType;

        public decimal Calculate(int insuranceLength, decimal basePremium)
        {
            var premium = basePremium * Multiplier;
            var maxDays = ToDay - FromDay;
            var daysCovered = Math.Min(insuranceLength-FromDay, maxDays);
            return daysCovered * premium;
        }
    }
    
    public const decimal BasePremium = 1250M; //ToDo: Should be data. 

    public decimal Calculate(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        return GetTranches(insuranceLength, coverType)
            .Select(x=> x.Calculate(insuranceLength, BasePremium))
            .Sum();
    }

    private IEnumerable<PremiumTranche> GetTranches(int insuranceLength, CoverType coverType)
    {
        var allCoverTypes = new PremiumTranche[]
        {
            new () { CoverType =  CoverType.Yacht, FromDay = 0, ToDay = 30,  Multiplier =  1.1m},
            new () { CoverType =  CoverType.Yacht, FromDay = 0, ToDay = 180,  Multiplier = 1.1m * (1- 0.5m)},
            new () { CoverType =  CoverType.Yacht, FromDay = 0, ToDay = 365,  Multiplier =  1.1m * (1- 0.3m)},
            new () { CoverType =  CoverType.PassengerShip, FromDay = 0, ToDay = 30,  Multiplier =  1.2m},
            new () { CoverType =  CoverType.PassengerShip, FromDay = 0, ToDay = 180,  Multiplier = 1.2m * (1- 0.2m)},
            new () { CoverType =  CoverType.PassengerShip, FromDay = 0, ToDay = 365,  Multiplier =  1.2m * (1- 0.8m)},
            new () { CoverType =  CoverType.Tanker, FromDay = 0, ToDay = 30,  Multiplier =  1.5m},
            new () { CoverType =  CoverType.Tanker, FromDay = 0, ToDay = 180,  Multiplier = 1.5m * (1- 0.2m)},
            new () { CoverType =  CoverType.Tanker, FromDay = 0, ToDay = 365,  Multiplier =  1.5m * (1- 0.8m)},
            new () { FromDay = 0, ToDay = 30,  Multiplier =  1.5m},
            new () { FromDay = 0, ToDay = 180,  Multiplier = 1.5m * (1- 0.2m)},
            new () { FromDay = 0, ToDay = 365,  Multiplier =  1.5m * (1- 0.8m)},
        };
        
        return allCoverTypes.Where(x=>  (x.CoverType ?? coverType) == coverType);
    }
}

/// <summary>
/// Copy/Paste of CoversController.ComputePremium, magic numbers and bugs included!
/// </summary>
public class BadPremiumCalculator: IPremiumCalculator
{
    
    public decimal Calculate(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var multiplier = 1.3m;
        if (coverType == CoverType.Yacht)
        {
            multiplier = 1.1m;
        }

        if (coverType == CoverType.PassengerShip)
        {
            multiplier = 1.2m;
        }

        if (coverType == CoverType.Tanker)
        {
            multiplier = 1.5m;
        }

        var premiumPerDay = 1250 * multiplier;
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        var totalPremium = 0m;

        //Tranched. Tranches vary-based on CoverType 
        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30) totalPremium += premiumPerDay;
            if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
        }

        return totalPremium;
    }
    
}