using System.Text.Json;
using Claims.Tests.Fixtures;
using Xunit;

namespace Claims.Tests
{
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
            var claims = JsonSerializer.Deserialize<List<Claim>>(jsonText);
            Assert.NotNull(claims); 
            Assert.Empty(claims);
        }


        [Theory]
        [InlineData("123", "456", "2025-01-01", 1000000, ClaimType.Collision)]
        public async Task Add_Claim_And_Check_If_Present(string id, string coverId, string createdDate, 
            decimal damageCost, ClaimType claimType)
        {
            var claim = new Claim()
            {
                Id = id,
                CoverId = coverId,
                Created = DateOnly.Parse(createdDate),
                DamageCost = damageCost,
                Type = claimType,
            };
            
            var client = _appFixture.GetHttpClient();
            
            var response = await client.PostAsync("/Claims", new StringContent(JsonSerializer.Serialize(claim)), TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();
            
           //ToDO: Make sure that this actually got saved.
        }
        

    }
}
