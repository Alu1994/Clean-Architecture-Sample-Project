using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Tests.Integration.Setups;

public class AuthenticationWebApplicationFactory : WebApplicationFactory<Presentation.Authentication.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests");
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<AuthenticationDataContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5440;Username=postgres;Password=b{}Yp)m3(3XS~T7(gBCHKR;Database=dbauths"));
        });
    }

    private TokenResultResponse _tokenResultResponse = default!;

    internal async Task<TokenResultResponse?> GetToken(bool generateNewToken = false)
    {
        if (_tokenResultResponse is not null && generateNewToken is false) return _tokenResultResponse;

        var authClient = CreateDefaultClient();
        var payload = new
        {
            email = "string",
            password = "string",
            twoFactorCode = "string",
            twoFactorRecoveryCode = "string"
        };
        var authResponse = await authClient.PostAsJsonAsync("/login", payload);
        var tokenResponse = await authResponse.Content.ReadFromJsonAsync<TokenResultResponse>();

        _tokenResultResponse = tokenResponse!;
        return tokenResponse;
    }
}