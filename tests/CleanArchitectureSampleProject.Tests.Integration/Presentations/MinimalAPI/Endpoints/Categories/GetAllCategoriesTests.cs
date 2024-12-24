using CleanArchitectureSampleProject.Core.Application.Outputs.Products;

namespace CleanArchitectureSampleProject.Tests.Integration.Presentations.MinimalAPI.Endpoints.Categories;

public class GetAllCategoriesTests : IntegrationTestSetup
{
    private readonly HttpClient _externalClientAPI;

    public GetAllCategoriesTests()
    {
        _externalClientAPI = WiremockServer.CreateClient();
    }

    [Fact]
    public async Task GivenValidRequest_WhenGettingAllCategories_ShouldReturn200OK_2()
    {
        // Act
        var response = await MinimalApi.GetAsync("/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEndpoint_ReturnsExpectedResponse_2()
    {
        // Act
        var response = await MinimalApi.GetAsync("/category");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CategoryOutput[]>();
        result.Should().NotBeNull();
    }
}
