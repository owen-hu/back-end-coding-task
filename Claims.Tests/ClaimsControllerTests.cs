using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    [Trait("Category", "Integration")]
    public class ClaimsControllerTests: IClassFixture<AppFixture>
    {

        private readonly AppFixture _appFixture;
        public ClaimsControllerTests(AppFixture fixture)
        {
            _appFixture = fixture;
        }
        
        
        /// <summary>
        /// This list may or may not be empty!
        /// </summary>
        [Fact]
        public async Task Get_Claims_Returns_List()
        {
            var client = _appFixture.GetHttpClient();
            var response = await client.GetAsync("/Claims", TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();
            
            //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
            // IN this case, we assure that the expected response is an empty array.
            var jsonText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var claims = JsonSerializer.Deserialize<List<ClaimDto>>(jsonText);
            Assert.NotNull(claims); 
        }


        [Fact]
        public async Task Add_Claim_And_Check_If_Present()
        {
            //Add a cover first.
            var today = DateOnly.FromDateTime(DateTime.Today);
            var cover = await AddCover(today, today);
            
            var claim = new ClaimDto()
            {
                CoverId = cover.Id,
                Name="",
                CreatedDate = today,
                DamageCost = 1000,
                ClaimType = ClaimType.Collision,
            };
            
            var client = _appFixture.GetHttpClient();

            var serializedClaim = JsonSerializer.Serialize(claim);
            
            var response = await client.PostAsync("/Claims", new StringContent(serializedClaim, Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();
            
            var jsonText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var responseClaim = JsonSerializer.Deserialize<ClaimDto>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
            
            Assert.NotNull(responseClaim);
            Assert.Equal(cover.Id, responseClaim.CoverId);
            Assert.NotNull(responseClaim.Id);
        }

        private async Task<CoverDto> AddCover(DateOnly startDate, DateOnly endDate)
        {
            var cover = new CoverDto()
            {
                StartDate = startDate,
                EndDate = endDate,
                CoverType = CoverType.Yacht,
                Premium = 1000
            };
            var client = _appFixture.GetHttpClient();
            var serializedCover = JsonSerializer.Serialize(cover);

            var response = await client.PostAsync("/Covers", 
                new StringContent(serializedCover, Encoding.UTF8, "application/json"), 
                TestContext.Current.CancellationToken);
            var jsonText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

            return JsonSerializer.Deserialize<CoverDto>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
        }
        
    }
}
