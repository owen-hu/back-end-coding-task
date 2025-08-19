using System.Text;
using System.Text.Json;
using Claims.ApiLayer;
using Claims.Core;
using Claims.Tests.Fixtures;
using Xunit;

namespace Claims.Tests
{
    /// <summary>
    /// Validate that ClaimsController REST end-points.
    /// </summary>
    /// <remarks>
    /// Integration Test, not a Unit test
    /// </remarks>
    public class ClaimsControllerTests: IClassFixture<AppFixture>
    {

        private readonly AppFixture _appFixture;
        public ClaimsControllerTests(AppFixture fixture)
        {
            _appFixture = fixture;
        }
        
        [Fact]
        public async Task Get_Claims_Empty_Returns_Empty_List()
        {
            var client = _appFixture.GetHttpClient();
            var response = await client.GetAsync("/Claims", TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();
            
            //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
            var jsonText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var claims = JsonSerializer.Deserialize<List<ClaimDto>>(jsonText);
            Assert.NotNull(claims); 
            Assert.Empty(claims);
        }


        [Theory]
        [InlineData("456", "2025-01-01", 1000000, ClaimType.Collision)]
        public async Task Add_Claim_And_Check_If_Present(string coverId, string createdDate, 
            decimal damageCost, ClaimType claimType)
        {
            var claim = new ClaimDto()
            {
                CoverId = coverId,
                Name="",
                CreatedDate = DateOnly.Parse(createdDate),
                DamageCost = damageCost,
                ClaimType = claimType,
            };
            
            var client = _appFixture.GetHttpClient();
            
            var serializedClaim = JsonSerializer.Serialize(claim);
            
            var response = await client.PostAsync("/Claims", new StringContent(serializedClaim, Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();
            
            var jsonText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var responseClaim = JsonSerializer.Deserialize<ClaimDto>(jsonText);
            Assert.NotNull(responseClaim);
            Assert.Equal(claim.CoverId, responseClaim.Id);
        }
        

    }
}
