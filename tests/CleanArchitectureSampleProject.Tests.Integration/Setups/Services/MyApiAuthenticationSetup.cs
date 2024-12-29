namespace CleanArchitectureSampleProject.Tests.Integration.Setups.Services;

using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Text.Encodings.Web;

internal static class MyApiAuthenticationSetup
{
    internal const string DefaultScheme = "TestScheme";
    internal const string DefaultUser = "TestUser";
    private const string ExpectedAccess = "true";

    public static IServiceCollection AddMyApiMockedAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(defaultScheme: DefaultScheme).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(DefaultScheme, options => { });

        services.AddAuthorization(options =>
        {
            foreach (var policy in PolicyExtensions.Policies)
                options.AddPolicy(policy.Key, p => p.RequireClaim(policy.Value, ExpectedAccess));
        });

        return services;
    }

    private class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new Collection<Claim>();
            foreach (var policy in PolicyExtensions.Policies)
            {
                claims.Add(new Claim(policy.Value, ExpectedAccess));
            }
            
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, DefaultScheme);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
