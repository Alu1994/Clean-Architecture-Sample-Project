using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Frozen;
using System.Net;
using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Application.UseCases;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;

namespace TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Products;

public static class ProductsEndpoints
{
    public readonly struct Logging;
    private const string TagName = "Products";
    private const string Controller = "product";
    private const string ContentType = "application/json";
    private const int Success = StatusCodes.Status200OK;
    private const int BadRequest = StatusCodes.Status400BadRequest;

    public static WebApplication MapProducts(this WebApplication app)
    {
        app.MapGet($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, CancellationToken cancellation) =>
        {
            return await GetAllProducts(logger, productUseCases, cancellation);
        })
        .Produces<FrozenSet<Product>>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get All Products", TagName);

        app.MapGet($"/{Controller}/{{productId:Guid}}", async (ILogger<Logging> logger, IProductUseCases productUseCases, Guid productId, CancellationToken cancellation) =>
        {
            return await GetById(logger, productUseCases, productId, cancellation);
        })
        .Produces<Product>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get Product By Id", TagName);

        app.MapGet($"/{Controller}/by-name/{{productName}}", async (ILogger<Logging> logger, IProductUseCases productUseCases, string productName, CancellationToken cancellation) =>
        {
            return await GetByName(logger, productUseCases, productName, cancellation);
        })
        .Produces<Product>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get Product By Name", TagName);

        app.MapPost($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation) =>
        {
            return await CreateProduct(logger, productUseCases, product, cancellation);
        })
        .Accepts<CreateProductInput>(ContentType)
        .Produces<Product>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Create Product", TagName);

        app.MapPut($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation) =>
        {
            return await UpdateProduct(logger, productUseCases, product, cancellation);
        })
        .Accepts<CreateProductInput>(ContentType)
        .Produces<Product>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Update Product", TagName);

        return app;
    }

    public static async Task<IResult> GetAllProducts(ILogger<Logging> logger, IProductUseCases productUseCases, CancellationToken cancellation)
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

    public static async Task<IResult> GetById(ILogger<Logging> logger, IProductUseCases productUseCases, Guid productId, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting product by id.";

        var result = await productUseCases.GetProductById(productId, cancellation);
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

    public static async Task<IResult> GetByName(ILogger<Logging> logger, IProductUseCases productUseCases, string productName, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting product by name.";

        var result = await productUseCases.GetProductByName(productName, cancellation);
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

    public static async Task<IResult> CreateProduct(ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation)
    {
        const string errorTitle = "Error while creating new product.";

        var result = await productUseCases.CreateProduct(product, cancellation);
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

    public static async Task<IResult> UpdateProduct(ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation)
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
