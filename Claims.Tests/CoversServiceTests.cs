using Claims.ApiLayer;
using Claims.Core;
using Claims.DataLayer.Claims;
using Moq;
using Xunit;

namespace Claims.Tests;

public class CoversServiceTests
{
    [Theory]
    [MemberData(nameof(GetInvalidCovers))]
    public async Task Add_Claim_FailsValidation(CoverDto coverDto, string expectedDescription)
    {
        //AAA pattern:
        
        //Arrange:
        var mockCoversRepo = new Mock<IRepository<Cover>>();
        mockCoversRepo.Setup(x=> x.AddItemAsync(It.IsAny<Cover>())).Throws(new Exception("Should never try and add this."));
        
        var auditor = new Mock<IAuditor>();
        var mockPremiumCalculator = new Mock<IPremiumCalculator>();
        //Act
        var coversService = new CoversService(mockCoversRepo.Object, auditor.Object, mockPremiumCalculator.Object);
        var ex = await Assert.ThrowsAnyAsync<Exception>( async () =>
        {
            _ = await coversService.CreateAndAuditAsync(coverDto);
        });  
        
        //Assert
        Assert.Equal(expectedDescription, ex.Message);
    }

    public static IEnumerable<object?[]> GetInvalidCovers()
    {
        yield return
        [
            new CoverDto
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(400)),
                CoverType = CoverType.Yacht,
            },
            "total insurance period cannot exceed 1 year"
        ];

        yield return
        [
            new CoverDto
            {
                StartDate = DateOnly.Parse("2001-01-01"),
                EndDate = DateOnly.Parse("2001-03-01"),
                CoverType = CoverType.Yacht
            },
            "StartDate cannot be in the past"
        ];
    }
    
}