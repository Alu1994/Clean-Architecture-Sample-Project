using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Setups;

public static class AuthenticationSetup
{
    public static string CategoryPolicy = "CategoryPolicy";
    private static readonly string CategoryClaim = "categoryclaim";

    public static string ProductPolicy = "ProductPolicy";
    private static readonly string ProductClaim = "productclaim";

    private static readonly string ValidAudience = "https://cleanarchsampleproject.com.br";
    private static readonly string ValidIssuer = "https://id.cleanarchsampleproject.com.br";
    private static readonly byte[] SymmetricKey = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(SymmetricKey),
            ValidIssuer = ValidIssuer,
            ValidAudience = ValidAudience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.FromSeconds(5),
        };

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions => jwtBearerOptions.TokenValidationParameters = tokenValidationParameters);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(CategoryPolicy, p => p.RequireClaim(CategoryClaim, "true"));
            options.AddPolicy(ProductPolicy, p => p.RequireClaim(ProductClaim, "true"));
        });

        return services;
    }
}