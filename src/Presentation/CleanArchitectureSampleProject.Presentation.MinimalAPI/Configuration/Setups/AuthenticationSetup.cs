using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Setups;

public static class AuthenticationSetup
{
    private const string ExpectedAccess = "true";

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        var tokenValidationParameters = CreateTokenValidationParameters();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions => jwtBearerOptions.TokenValidationParameters = tokenValidationParameters);

        services.AddAuthorization(options =>
        {
            foreach (var policy in Policies)
                options.AddPolicy(policy.Key, p => p.RequireClaim(policy.Value, ExpectedAccess));
        });

        return services;
    }
}