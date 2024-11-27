using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Setups;

public static class AuthenticationSetup
{

    public static string MyPolicyName = "MyPolicyName";
    public static string myclaimname = "myclaimname";

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey("MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray()),
                        ValidIssuer = "https://id.cleanarchsampleproject.com.br",
                        ValidAudience = "https://cleanarchsampleproject.com.br",
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                    };
            });

        services.AddAuthorization();
        //services.AddAuthorization(
        //    //options =>
        //    //    options.AddPolicy(MyPolicyName, p => 
        //    //        p.RequireClaim(myclaimname, "true"))
        //    );

        return services;
    }
}