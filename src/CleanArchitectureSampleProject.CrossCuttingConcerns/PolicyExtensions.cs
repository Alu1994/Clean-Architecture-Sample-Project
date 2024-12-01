using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class PolicyExtensions
{
    public const string CategoryCanReadPolicy = "CategoryCanReadPolicy";
    public const string CategoryCanWritePolicy = "CategoryCanWritePolicy";
    public const string CategoryCanDeletePolicy = "CategoryCanDeletePolicy";
    public const string ProductCanReadPolicy = "ProductCanReadPolicy";
    public const string ProductCanWritePolicy = "ProductCanWritePolicy";
    public const string ProductCanDeletePolicy = "ProductCanDeletePolicy";
    public const string SellCanReadPolicy = "SellCanReadPolicy";
    public const string SellCanWritePolicy = "SellCanWritePolicy";
    public const string SellCanDeletePolicy = "SellCanDeletePolicy";

    private const byte TokenDelayAccess = 5;
    private const byte DefaultExpiresIn = 60;
    private const string ValidAudience = "https://cleanarchsampleproject.com.br";
    private const string ValidIssuer = "https://id.cleanarchsampleproject.com.br";
    private static readonly byte[] SymmetricKey = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();

    public static IReadOnlyDictionary<string, string> Policies => new Dictionary<string, string>
    {
        [CategoryCanReadPolicy] = "categorycanreadclaim",
        [CategoryCanWritePolicy] = "categorycanwriteclaim",
        [CategoryCanDeletePolicy] = "categorycandeleteclaim",
        [ProductCanReadPolicy] = "productcanreadclaim",
        [ProductCanWritePolicy] = "productcanwriteclaim",
        [ProductCanDeletePolicy] = "productcandeleteclaim",
        [SellCanReadPolicy] = "sellcanreadclaim",
        [SellCanWritePolicy] = "sellcanwriteclaim",
        [SellCanDeletePolicy] = "sellcandeleteclaim"
    };

    public static (string, byte, SecurityToken) ToSecurityTokenDescriptor(this List<Claim> claims, byte expiresIn = DefaultExpiresIn)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(SymmetricKey);
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiresIn),
            Issuer = ValidIssuer,
            Audience = ValidAudience,
            NotBefore = DateTime.UtcNow.Add(TimeSpan.FromSeconds(TokenDelayAccess)),
            SigningCredentials = signingCredentials,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var tokenValue = tokenHandler.WriteToken(securityToken);

        return (tokenValue, expiresIn, securityToken);
    }

    public static TokenValidationParameters CreateTokenValidationParameters()
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
            ClockSkew = TimeSpan.FromSeconds(TokenDelayAccess),
        };

        return tokenValidationParameters;
    }
}
