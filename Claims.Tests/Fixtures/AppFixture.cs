using Microsoft.AspNetCore.Mvc.Testing;

namespace Claims.Tests.Fixtures;

public class AppFixture
{
    private readonly Lazy<HttpClient> _client =
        new Lazy<HttpClient>(() =>
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                    {});
                
            return application.CreateClient();
        });

    public HttpClient GetHttpClient()
    {
        return _client.Value; 
    }
}