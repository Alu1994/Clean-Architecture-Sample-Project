using TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Categories;
using TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Products;

namespace TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints;

public static class MainEndpointsMapping
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapProducts();
        app.MapCategories();

        return app;
    }
}
