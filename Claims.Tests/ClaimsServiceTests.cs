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
    public async Task Add_Claim_FailsValidation(CoverDto coverDto, ClaimDto claimDto, string expectedDescription)
    {
        var mockRepo = new Mock<IRepository<Claim>>();
        mockRepo.Setup(x=> x.AddItemAsync(It.IsAny<Claim>())).Throws(new Exception("Should never try and add this."));
        var auditor = new Mock<Auditor>();
        var claimsService = new ClaimsService(mockRepo.Object, auditor.Object);
        var ex = await Assert.ThrowsAnyAsync<Exception>( async () =>
        {
            _ = await claimsService.CreateAndAuditAsync(claimDto);
        });  
        
        Assert.Equal(expectedDescription, ex.Message);
    }


    public static IEnumerable<object[]> GetInvalidClaims()
    {
        //Damage cost above threshold
        yield return
        [
            new CoverDto()
            {
                Id = "123",
                StartDate = DateOnly.Parse("2025-01-01"),
                EndDate = DateOnly.Parse("2025-04-01"),
                CoverType = CoverType.Yacht,
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
            new CoverDto()
            {
                Id = "123",
                StartDate = DateOnly.Parse("2025-01-01"),
                EndDate = DateOnly.Parse("2025-04-01"),
                CoverType = CoverType.Yacht,
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
    }
}