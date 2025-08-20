namespace Claims.Core;

/// <summary>
/// Logic seems to be based on Tranches.
/// </summary>
public class TranchedPremiumCalculator : IPremiumCalculator
{
    private class PremiumTranche
    {
        public decimal Multiplier;
        public int FromDayNo;
        public int? ToDayNo;
        
        /// <summary>
        /// If not set, assume that this is the default.
        /// </summary>
        public CoverType? CoverType;

        /// <summary>
        /// Get the slice of the premium that applies to this tranche, or 0 if it doesn't apply
        /// </summary>
        /// <param name="insuranceLength"></param>
        /// <param name="basePremium"></param>
        /// <returns></returns>
        public decimal Calculate(int insuranceLength, decimal basePremium)
        {
            if (insuranceLength < FromDayNo)
            {
                return 0;
            }
            
            var premium = basePremium * Multiplier;
            var maxDays = (ToDayNo ?? int.MaxValue) - FromDayNo+1;
            var daysCovered = Math.Min(insuranceLength-FromDayNo+1, maxDays);
            return daysCovered * premium;
        }
    }
    
    public const decimal BasePremium = 1250M; //ToDo: Should be data. 

    public decimal Calculate(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        if (endDate.DayNumber < startDate.DayNumber)
        {
            throw new ArgumentException("endDate must be greater than startDate");
        }

        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        return GetTranches(coverType)
            .Select(x=> x.Calculate(insuranceLength, BasePremium))
            .Sum();
    }

    /// <summary>
    /// In reality, this should come from a data-source (e.g. DB).
    /// </summary>
    /// <param name="coverType"></param>
    /// <returns></returns>
    private IEnumerable<PremiumTranche> GetTranches(CoverType coverType)
    {
        var allCoverTypes = new PremiumTranche[]
        {
            new () { CoverType =  CoverType.Yacht, FromDayNo = 1, ToDayNo = 30,  Multiplier =  1.1m},
            new () { CoverType =  CoverType.Yacht, FromDayNo = 31, ToDayNo = 180,  Multiplier = 1.1m * (1- 0.05m)},
            new () { CoverType =  CoverType.Yacht, FromDayNo = 181,  Multiplier =  1.1m * (1- 0.08m)},
            new () { CoverType =  CoverType.PassengerShip, FromDayNo = 1, ToDayNo = 30,  Multiplier =  1.2m},
            new () { CoverType =  CoverType.PassengerShip, FromDayNo = 31, ToDayNo = 180,  Multiplier = 1.2m * (1- 0.02m)},
            new () { CoverType =  CoverType.PassengerShip, FromDayNo = 181, Multiplier =  1.2m * (1- 0.03m)},
            new () { CoverType =  CoverType.Tanker, FromDayNo = 1, ToDayNo = 30,  Multiplier =  1.5m},
            new () { CoverType =  CoverType.Tanker, FromDayNo = 31, ToDayNo = 180,  Multiplier = 1.5m * (1- 0.02m)},
            new () { CoverType =  CoverType.Tanker, FromDayNo = 181, Multiplier =  1.5m * (1- 0.03m)},
            new () { FromDayNo = 1, ToDayNo = 30,  Multiplier =  1.3m},
            new () { FromDayNo = 31, ToDayNo = 180,  Multiplier = 1.3m * (1- 0.02m)},
            new () { FromDayNo = 181, Multiplier =  1.3m * (1- 0.03m)},
        };
        
        
        var coverTypes = allCoverTypes.Where(x => x.CoverType == coverType).ToList();
        if (coverTypes.Any())
        {
            return coverTypes;
        }
        
        return allCoverTypes.Where(x=>  x.CoverType is null);
    }
}