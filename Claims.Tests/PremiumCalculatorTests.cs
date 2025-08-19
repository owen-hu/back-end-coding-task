using Claims.Core;
using Xunit;

namespace Claims.Tests;

/// <summary>
/// Tests for Task 5.
/// </summary>
public abstract class PremiumCalculatorTests
{

    /// <summary>
    /// These are the Tests for the Original Premium Calculator.
    /// It's pretty good at calculating a zero-day premium. 
    /// </summary>
    public class BadPremiumCalculatorTests: PremiumCalculatorTests
    {
        protected override IPremiumCalculator CreateCalculator()
        {
            return new BadPremiumCalculator();
        }
    }

    /// <summary>
    /// This is the "Fixed" Premium Calculator, 
    /// </summary>
    public class TranchedPremiumCalculatorTests: PremiumCalculatorTests
    {
        protected override IPremiumCalculator CreateCalculator()
        {
            return new TranchedPremiumCalculator();
        }
    }
    
    protected abstract IPremiumCalculator CreateCalculator();
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1250*1.1)] //10% more expensive
    [InlineData(2, 1250*1.1*2)] //2 days
    [InlineData(30, 1250*1.1*30)] //Formula for 30 days
    [InlineData(31, 1250*1.1*30 + 1250*1.1*0.95)] //31st day: 5% discount. 
    [InlineData(180, 1250*1.1*30 + 1250*1.1*0.95*150)] //up to 180th day: also 5% discount. 
    [InlineData(181, 1250*1.1*30 + 1250*1.1*0.95*150 + 1250*1.1*0.92)] //181st day: now at 8% discount (Additional 3%) 
    [InlineData(360, 1250*1.1*30 + 1250*1.1*0.95*150 + 1250*1.1*0.92*180)] //And every day after that. 
    public void Premium_For_Yachts_Should_Be(int duration, decimal expectedPremium)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var premiumCalculator = CreateCalculator();
        var premium = premiumCalculator.Calculate(startDate, startDate.AddDays(duration), CoverType.Yacht);
        Assert.Equal(expectedPremium, premium);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1250*1.2)] //10% more expensive
    [InlineData(2, 1250*1.2*2)] //2 days
    [InlineData(30, 1250*1.2*30)] //Formula for 30 days
    [InlineData(31, 1250*1.2*30 + 1250*1.2*0.98)] //31st day: 5% discount. 
    [InlineData(180, 1250*1.2*30 + 1250*1.2*0.98*150)] //up to 180th day: also 5% discount. 
    [InlineData(181, 1250*1.2*30 + 1250*1.2*0.98*150 + 1250*1.2*0.97)] //181st day: now at 8% discount (Additional 3%) 
    [InlineData(360, 1250*1.2*30 + 1250*1.2*0.98*150 + 1250*1.2*0.97*180)] //And every day after that. 
    public void Premium_For_Passenger_Ships_Should_Be(int duration, decimal expectedPremium)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var premiumCalculator = CreateCalculator();
        var premium = premiumCalculator.Calculate(startDate, startDate.AddDays(duration), CoverType.PassengerShip);
        Assert.Equal(expectedPremium, premium);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1250*1.5)] //10% more expensive
    [InlineData(2, 1250*1.5*2)] //2 days
    [InlineData(30, 1250*1.5*30)] //Formula for 30 days
    [InlineData(31, 1250*1.5*30 + 1250*1.5*0.98)] //31st day: 5% discount. 
    [InlineData(180, 1250*1.5*30 + 1250*1.5*0.98*150)] //up to 180th day: also 5% discount. 
    [InlineData(181, 1250*1.5*30 + 1250*1.5*0.98*150 + 1250*1.5*0.97)] //181st day: now at 8% discount (Additional 3%) 
    [InlineData(360, 1250*1.5*30 + 1250*1.5*0.98*150 + 1250*1.5*0.97*180)] //And every day after that. 
    public void Premium_For_Tankers_Should_Be(int duration, decimal expectedPremium)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var premiumCalculator = CreateCalculator();
        var premium = premiumCalculator.Calculate(startDate, startDate.AddDays(duration), CoverType.Tanker);
        Assert.Equal(expectedPremium, premium);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1250*1.3)] //10% more expensive
    [InlineData(2, 1250*1.3*2)] //2 days
    [InlineData(30, 1250*1.3*30)] //Formula for 30 days
    [InlineData(31, 1250*1.3*30 + 1250*1.3*0.98)] //31st day: 5% discount. 
    [InlineData(180, 1250*1.3*30 + 1250*1.3*0.98*150)] //up to 180th day: also 5% discount. 
    [InlineData(181, 1250*1.3*30 + 1250*1.3*0.98*150 + 1250*1.3*0.97)] //181st day: now at 8% discount (Additional 3%) 
    [InlineData(360, 1250*1.3*30 + 1250*1.3*0.98*150 + 1250*1.3*0.97*180)] //And every day after that. 
    public void Premium_For_Bulk_Carriers_Should_Be(int duration, decimal expectedPremium)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var premiumCalculator = CreateCalculator();
        var premium = premiumCalculator.Calculate(startDate, startDate.AddDays(duration), CoverType.BulkCarrier);
        Assert.Equal(expectedPremium, premium);
    }

    
    [Fact]
    public void Start_Date_After_EndDate_Should_Throw_Exception()
    {
        var premiumCalculator = CreateCalculator();
        Assert.ThrowsAny<Exception>(() =>
        {
            var premium = premiumCalculator.Calculate(DateOnly.Parse("2025-08-02"), DateOnly.Parse("2025-08-01"),
                CoverType.Yacht);
        });
    }
    
}

