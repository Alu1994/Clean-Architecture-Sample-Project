using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

namespace CleanArchitectureSampleProject.Tests.Integration;

public class UnitTest1(MinimalWebApplicationFactory webFactory, AuthenticationWebApplicationFactory authFactory) :
    IClassFixture<MinimalWebApplicationFactory>,
    IClassFixture<AuthenticationWebApplicationFactory>
{
    private readonly MinimalWebApplicationFactory _webFactory = webFactory;
    private readonly AuthenticationWebApplicationFactory _authFactory = authFactory;

    [Fact]
    public async Task GetEndpoint_ReturnsExpectedResponse()
    {
        // Arrange
        TokenResultResponse? tokenResponse = await _authFactory.GetToken();
        var client = _webFactory.CreateDefaultClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResponse!.AccessToken}");

        // Act
        var response = await client.GetAsync("/category");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<CategoryOutput>>();
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }
}
