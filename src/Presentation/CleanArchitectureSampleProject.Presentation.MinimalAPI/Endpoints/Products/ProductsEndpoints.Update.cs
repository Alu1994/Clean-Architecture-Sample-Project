using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.Application.UseCases;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static WebApplication MapUpdate(this WebApplication app)
    {
        app.MapPut($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation) =>
        {
            return await Update(logger, productUseCases, product, cancellation);
        })
        .Accepts<UpdateProductInput>(ContentType)
        .Produces<UpdateProductOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Update Product", TagName);
        return app;
    }

    private static async Task<IResult> Update(ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation)
    {
        const string errorTitle = "Error while updating new product.";

        var result = await productUseCases.UpdateProduct(product, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogSeqError(error);
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
