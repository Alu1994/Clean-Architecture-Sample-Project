using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

namespace CleanArchitectureSampleProject.Tests.Unit.Endpoints;

public sealed class CategoriesEndpointsCreateTests
{
    [Fact]
    public async Task Given_When_Then()
    {
        var x = CategoriesEndpoints.Create(null, null, null, CancellationToken.None);
    }
}
