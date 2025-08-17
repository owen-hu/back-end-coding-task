using System.Text.Json;
using Claims.ApiLayer;
using Claims.Tests.Fixtures;
using Xunit;

namespace Claims.Tests;

public class CoversControllerTests: IClassFixture<AppFixture>
{
    private readonly AppFixture _appFixture;
    public CoversControllerTests(AppFixture fixture)
    {
        _appFixture = fixture;
    }
    
    [Fact]
    public async Task Get_Covers_Empty_Returns_Empty_List()
    {
        var client = _appFixture.GetHttpClient();
        var response = await client.GetAsync("/Covers", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
            
        //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
        var jsonText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var claims = JsonSerializer.Deserialize<List<CoverDto>>(jsonText);
        Assert.NotNull(claims); 
        Assert.Empty(claims);
    }
}