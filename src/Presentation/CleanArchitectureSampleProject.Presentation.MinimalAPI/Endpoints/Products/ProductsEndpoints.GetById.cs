namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static WebApplication MapGetById(this WebApplication app)
    {
        app.MapGet($"/{Controller}/{{productId:Guid}}", async (ILogger<Logging> logger, IProductUseCases productUseCases, Guid productId, CancellationToken cancellation) =>
        {
            return await GetById(logger, productUseCases, productId, cancellation);
        })
        .Produces<GetProductOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .Produces<UnauthorizedResponse>(Unauthorized, ContentType)
        .WithConfigSummaryInfo("Get Product By Id", TagName)
        .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetById(ILogger<Logging> logger, IProductUseCases productUseCases, Guid productId, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting product by id.";

        var result = await productUseCases.GetProductById(productId, cancellation);
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
