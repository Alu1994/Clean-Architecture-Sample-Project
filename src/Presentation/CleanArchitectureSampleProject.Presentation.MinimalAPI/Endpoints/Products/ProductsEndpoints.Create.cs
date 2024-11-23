using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.Application.UseCases;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static WebApplication MapCreate(this WebApplication app)
    {
        app.MapPost($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation) =>
        {
            return await Create(logger, productUseCases, product, cancellation);
        })
        .Accepts<CreateProductInput>(ContentType)
        .Produces<CreateProductOutput>(Created, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Create Product", TagName);
        return app;
    }

    private static async Task<IResult> Create(ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation)
    {
        const string errorTitle = "Error while creating new product.";

        var result = await productUseCases.CreateProduct(product, cancellation);
        return result.Match(success => Results.Created($"/{Controller}", success),
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
