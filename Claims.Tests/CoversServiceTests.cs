using Claims.ApiLayer;
using Claims.Core;
using Claims.DataLayer.Claims;
using Moq;
using Xunit;

namespace Claims.Tests;

[Trait("Category", "Unit")]
public class CoversServiceTests
{
    [Fact]
    public async Task Failing_Cover_Does_Not_Add_Item()
    {
        //AAA pattern:
        
        //Arrange:
        var auditor = new Mock<IAuditor>();
        var mockCoversRepo = new Mock<IRepository<Cover>>();
        var mockPremiumCalculator = new Mock<IPremiumCalculator>();
        var mockValidator = new Mock<IValidator<CoverDto>>();
        
        mockCoversRepo.Setup(x=> x.AddItemAsync(It.IsAny<Cover>()))
            .Throws(new Exception("Should never try and add this."));

        mockValidator.Setup(x => x.ValidateAsync(It.IsAny<CoverDto>()))
            .ThrowsAsync(new ValidationException("FAIL"));
        
        //Act
        var coversService = new CoversService(mockCoversRepo.Object, auditor.Object, mockPremiumCalculator.Object, mockValidator.Object);
        var ex = await Assert.ThrowsAnyAsync<ValidationException>( async () =>
        {
            _ = await coversService.CreateAndAuditAsync(new CoverDto());
        });  
        
        //Assert
        Assert.Equal("FAIL", ex.Message);
    }
}