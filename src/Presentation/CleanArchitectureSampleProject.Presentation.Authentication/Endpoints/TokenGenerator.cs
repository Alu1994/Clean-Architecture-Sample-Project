using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;

public sealed class TokenGenerator
{
    private readonly IUserRepository _userRepository;

    public TokenGenerator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Results<string, BaseError>> GenerateToken(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetUserByEmailAndPassword(request.Email, request.Password, cancellationToken);
        if (result.IsFail) return result.Error!;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "https://id.cleanarchsampleproject.com.br",
            Audience = "https://cleanarchsampleproject.com.br",
            NotBefore = DateTime.UtcNow.Add(TimeSpan.FromSeconds(60)),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<User> CreateUser(User user, CancellationToken cancellationToken)
    {
        var isSuccess = await _userRepository.Insert(user, cancellationToken);

        return user;
    }
}
