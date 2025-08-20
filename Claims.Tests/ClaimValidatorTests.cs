using Claims.ApiLayer;
using Claims.Core;
using Claims.DataLayer.Claims;
using Moq;
using Xunit;

namespace Claims.Tests;

public class ClaimValidatorTests
{
    [Theory]
    [MemberData(nameof(GetInvalidClaims))]
    public async Task Invalid_Claim_Fails_Validation(Cover? cover, ClaimDto claimDto, string expectedDescription)
    {
        //AAA pattern:
        
        //Arrange:
        var mockCoversRepo = new Mock<IRepository<Cover>>();
        if (cover != null)
        {
            mockCoversRepo.Setup(x=> x.GetItemAsync(cover.Id)).ReturnsAsync(cover);
        }
        
        var validator = new ClaimValidator(mockCoversRepo.Object);
        
        //Act
        var ex = await Assert.ThrowsAnyAsync<ValidationException>( async () =>
        {
            await validator.ValidateAsync(claimDto);
        });  
        
        //Assert
        Assert.Equal(expectedDescription, ex.Message);
    }

    [Fact]
    public async Task Valid_Claim_Passes_Validation()
    {
        var mockCoversRepo = new Mock<IRepository<Cover>>();
        
        mockCoversRepo.Setup(x=> x.GetItemAsync("123")).ReturnsAsync(new Cover()
        {
            Id = "123",
            StartDate = DateOnly.Parse("2025-01-01"),
            EndDate = DateOnly.Parse("2025-03-01"),
            Premium = 100000,
            Type = CoverType.Yacht
        });
        var validator = new ClaimValidator(mockCoversRepo.Object);
        await validator.ValidateAsync(new ClaimDto()
        {
            Id = "1234",
            CoverId = "123",
            DamageCost = 1000,
            Name = "Test claim",
            CreatedDate = DateOnly.Parse("2025-01-03"),
            ClaimType = ClaimType.Collision
        });
        
        Assert.True(true, "If we get here, no Validation exception was thrown");
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
                DamageCost = 100M,
                CoverId = "123",
                CreatedDate = DateOnly.Parse("2025-04-02")
            }, 
            "Created date must be within the period of the related Cover"
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
            "Cover not found for claim"
        ];
    }
}

public class CoverValidatorTests
{
    private class TestCoverValidator : CoverValidator
    {
        public DateTime? OverrideToday {get; set;}
        protected override DateTime Today => OverrideToday ?? base.Today;
    }
    
    [Theory]
    [MemberData(nameof(GetInvalidCovers))]
    public async Task Invalid_Cover_Fails_Validation(CoverDto coverDto, string expectedDescription)
    {
        //AAA pattern:
        
        //Arrange:
        var coverValidator = new TestCoverValidator();
        
        //Act
        var ex = await Assert.ThrowsAnyAsync<Exception>( async () =>
        {
            await coverValidator.ValidateAsync(coverDto);
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