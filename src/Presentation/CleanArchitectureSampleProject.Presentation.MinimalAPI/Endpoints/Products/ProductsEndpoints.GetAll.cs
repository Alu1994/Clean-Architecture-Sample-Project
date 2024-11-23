using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.Application.UseCases;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Frozen;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static WebApplication MapGetAll(this WebApplication app)
    {
        app.MapGet($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, CancellationToken cancellation) =>
        {
            return await GetAll(logger, productUseCases, cancellation);
        })
        .Produces<FrozenSet<GetProductOutput>>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get All Products", TagName);
        return app;
    }

    private static async Task<IResult> GetAll(ILogger<Logging> logger, IProductUseCases productUseCases, CancellationToken cancellation)
    {
        var result = await productUseCases.GetProducts(cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogSeqError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: "Error",
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }

}
