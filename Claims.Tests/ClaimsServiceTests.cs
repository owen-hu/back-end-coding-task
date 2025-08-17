using Claims.ApiLayer;
using Claims.Core;
using Claims.DataLayer.Claims;
using Xunit;
using Moq;

namespace Claims.Tests;

public class ClaimsServiceTests
{

    [Theory]
    [MemberData(nameof(GetInvalidClaims))]
    public async Task Add_Claim_FailsValidation(Cover? cover, ClaimDto claimDto, string expectedDescription)
    {
        //AAA pattern:
        
        //Arrange:
        var mockClaimsRepo = new Mock<IRepository<Claim>>();
        mockClaimsRepo.Setup(x=> x.AddItemAsync(It.IsAny<Claim>())).Throws(new Exception("Should never try and add this."));
        
        var mockCoversRepo = new Mock<IRepository<Cover>>();

        if (cover != null)
        {
            mockCoversRepo.Setup(x=> x.GetItemAsync(cover.Id)).ReturnsAsync(cover);
        }

        var auditor = new Mock<IAuditor>();
        
        //Act
        var claimsService = new ClaimsService(mockClaimsRepo.Object, mockCoversRepo.Object, auditor.Object);
        var ex = await Assert.ThrowsAnyAsync<Exception>( async () =>
        {
            _ = await claimsService.CreateAndAuditAsync(claimDto);
        });  
        
        //Assert
        Assert.Equal(expectedDescription, ex.Message);
    }


    public static IEnumerable<object?[]> GetInvalidClaims()
    {
        //Damage cost above threshold
        yield return
        [
            new Cover()
            {
                Id = "123",
                StartDate = DateOnly.Parse("2025-01-01"),
                EndDate = DateOnly.Parse("2025-04-01"),
                Type = CoverType.Yacht,
                Premium = 10000
            },
            new ClaimDto()
            {
                ClaimType = ClaimType.Collision,
                DamageCost = 100001M,
                CoverId = "123",
                CreatedDate = DateOnly.Parse("2025-01-02")
            }, 
            "DamageCost cannot exceed 100,000"
        ];
        
        yield return
        [
            new Cover()
            {
                Id = "123",
                StartDate = DateOnly.Parse("2025-01-01"),
                EndDate = DateOnly.Parse("2025-04-01"),
                Type = CoverType.Yacht,
                Premium = 10000
            },
            new ClaimDto()
            {
                ClaimType = ClaimType.Collision,
                DamageCost = 100001M,
                CoverId = "123",
                CreatedDate = DateOnly.Parse("2025-04-02")
            }, 
            "Claim date 2025-04-02 is not within the cover range: 2025-01-01 to 2025-04-01"
        ];
        
        yield return
        [
            null,
            new ClaimDto()
            {
                ClaimType = ClaimType.Collision,
                DamageCost = 100001M,
                CoverId = "123",
                CreatedDate = DateOnly.Parse("2025-04-02")
            }, 
            "Claim date 2025-04-02 is not within the cover range: 2025-01-01 to 2025-04-01"
        ];
    }
}