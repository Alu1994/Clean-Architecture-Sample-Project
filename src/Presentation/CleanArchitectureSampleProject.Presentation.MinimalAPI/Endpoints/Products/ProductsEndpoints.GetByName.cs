using CleanArchitectureSampleProject.Core.Application.Outputs;
using CleanArchitectureSampleProject.Core.Application.UseCases;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/by-name/{{productName}}", async (ILogger<Logging> logger, IProductUseCases productUseCases, string productName, CancellationToken cancellation) =>
        {
            return await GetByName(logger, productUseCases, productName, cancellation);
        })
        .Produces<GetProductOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get Product By Name", TagName);
        return app;
    }

    private static async Task<IResult> GetByName(ILogger<Logging> logger, IProductUseCases productUseCases, string productName, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting product by name.";

        var result = await productUseCases.GetProductByName(productName, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogBaseError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitle,
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }

}
