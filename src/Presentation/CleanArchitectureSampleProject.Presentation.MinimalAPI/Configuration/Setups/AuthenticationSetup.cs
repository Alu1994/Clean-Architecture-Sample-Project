using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Setups;

public static class AuthenticationSetup
{
    public static readonly string CategoryCanReadPolicy = "CategoryCanReadPolicy";
    public static readonly string CategoryCanWritePolicy = "CategoryCanWritePolicy";
    public static readonly string CategoryCanDeletePolicy = "CategoryCanDeletePolicy";
    public static readonly string ProductCanReadPolicy = "ProductCanReadPolicy";
    public static readonly string ProductCanWritePolicy = "ProductCanWritePolicy";
    public static readonly string ProductCanDeletePolicy = "ProductCanDeletePolicy";

    private static readonly byte[] SymmetricKey = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();
    private const string ExpectedAccess = "true";
    private const string ValidAudience = "https://cleanarchsampleproject.com.br";
    private const string ValidIssuer = "https://id.cleanarchsampleproject.com.br";
    
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
            foreach (var policy in Policies)
                options.AddPolicy(policy.Key, p => p.RequireClaim(policy.Value, ExpectedAccess));
        });

        return services;
    }

    private static IReadOnlyDictionary<string, string> Policies => new Dictionary<string, string>
    {
        [CategoryCanReadPolicy] = "categorycanreadclaim",
        [CategoryCanWritePolicy] = "categorycanwriteclaim",
        [CategoryCanDeletePolicy] = "categorycandeleteclaim",
        [ProductCanReadPolicy] = "productcanreadclaim",
        [ProductCanWritePolicy] = "productcanwriteclaim",
        [ProductCanDeletePolicy] = "productcandeleteclaim"
    };
}