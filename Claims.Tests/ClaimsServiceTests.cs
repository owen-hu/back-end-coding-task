using Claims.ApiLayer;
using Claims.Core;
using Claims.DataLayer.Claims;
using Xunit;
using Moq;

namespace Claims.Tests;

[Trait("Category", "Unit")]
public class ClaimsServiceTests
{

    [Fact]
    public async Task Claim_That_Fails_Validation_Should_Not_Add_Or_Audit()
    {
        //AAA pattern:
        
        //Arrange:
        var mockClaimsRepo = new Mock<IRepository<Claim>>();
        var auditor = new Mock<IAuditor>();
        var validator = new Mock<IValidator<ClaimDto>>();

        //Make sure we don't try and add it.
        mockClaimsRepo.Setup(x=> x.AddItemAsync(It.IsAny<Claim>()))
            .Throws(new Exception("Should never try and add this."));

        //Make sure we don't try and audit it.
        auditor.Setup(x=> x.AuditClaim(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Should never try and audit this."));

        //Validator should always throw "FAIL"
        validator.Setup( x=> x.ValidateAsync(It.IsAny<ClaimDto>()))
            .ThrowsAsync(new ValidationException("FAIL"));
        
        var claimsService = new ClaimsService(mockClaimsRepo.Object, auditor.Object, validator.Object);

        
        //Act
        var ex = await Assert.ThrowsAnyAsync<ValidationException>( async () =>
        {
            _ = await claimsService.CreateAndAuditAsync(new ClaimDto());
        });  
        
        //Assert
        Assert.Equal("FAIL", ex.Message);
    }
}