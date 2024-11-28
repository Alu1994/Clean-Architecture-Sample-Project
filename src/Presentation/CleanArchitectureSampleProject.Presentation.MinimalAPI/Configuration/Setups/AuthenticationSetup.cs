using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Setups;

public static class AuthenticationSetup
{
    public static string CategoryPolicyCanRead = "CategoryPolicyCanRead";
    public static string CategoryPolicyCanWrite = "CategoryPolicyCanWrite";
    public static string CategoryPolicyCanDelete = "CategoryPolicyCanDelete";
    private static readonly string CategoryClaimCanRead = "categorycanreadclaim";
    private static readonly string CategoryClaimCanWrite = "categorycanwriteclaim";
    private static readonly string CategoryClaimCanDelete = "categorycandeleteclaim";

    public static string ProductPolicyCanRead = "ProductPolicyCanRead";
    public static string ProductPolicyCanWrite = "ProductPolicyCanWrite";
    public static string ProductPolicyCanDelete = "ProductPolicyCanDelete";
    private static readonly string ProductClaimCanRead = "productcanreadclaim";    
    private static readonly string ProductClaimCanWrite = "productcanwriteclaim";    
    private static readonly string ProductClaimCanDelete = "productcandeleteclaim";

    private static readonly string ExpectedAccess = "True";

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
            options.AddPolicy(CategoryPolicyCanRead, p => p.RequireClaim(CategoryClaimCanRead, ExpectedAccess));
            options.AddPolicy(CategoryPolicyCanWrite, p => p.RequireClaim(CategoryClaimCanWrite, ExpectedAccess));
            options.AddPolicy(CategoryPolicyCanDelete, p => p.RequireClaim(CategoryClaimCanDelete, ExpectedAccess));

            options.AddPolicy(ProductPolicyCanRead, p => p.RequireClaim(ProductClaimCanRead, ExpectedAccess));            
            options.AddPolicy(ProductPolicyCanWrite, p => p.RequireClaim(ProductClaimCanWrite, ExpectedAccess));            
            options.AddPolicy(ProductPolicyCanDelete, p => p.RequireClaim(ProductClaimCanDelete, ExpectedAccess));
        });

        return services;
    }
}