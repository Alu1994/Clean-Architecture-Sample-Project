using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;

public sealed class TokenGenerator(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private static readonly byte[] SymmetricKey = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();

    public async Task<Results<string, BaseError>> GenerateToken(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetUserByEmailAndPassword(request.Email, request.Password, cancellationToken);
        if (result.IsFail) return result.Error!;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new("categoryclaim", "true"),
            new("productclaim", "true"),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "https://id.cleanarchsampleproject.com.br",
            Audience = "https://cleanarchsampleproject.com.br",
            NotBefore = DateTime.UtcNow.Add(TimeSpan.FromSeconds(5)),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(SymmetricKey),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<User> CreateUser(User user, CancellationToken cancellationToken)
    {
        var isSuccess = await _userRepository.Insert(user, cancellationToken);
        return user;
    }
}
