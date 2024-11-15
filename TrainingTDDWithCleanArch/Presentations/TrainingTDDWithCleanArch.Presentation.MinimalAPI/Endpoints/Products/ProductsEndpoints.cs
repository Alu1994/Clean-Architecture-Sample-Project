using Microsoft.AspNetCore.Mvc;
using System.Collections.Frozen;
using System.Net;
using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;

namespace TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Products;

public static class ProductsEndpoints
{
    private const string TagName = "Products";
    private const string Controller = "product";
    private const string ContentType = "application/json";
    private const int Success = StatusCodes.Status200OK;
    private const int BadRequest = StatusCodes.Status400BadRequest;

    public static WebApplication MapProducts(this WebApplication app)
    {
        app.MapGet($"/{Controller}", async (IProductUseCases productUseCases, CancellationToken cancellation) => 
        { 
            return await GetAllProducts(productUseCases, cancellation); 
        })
        .Produces(Success, typeof(FrozenSet<Product>), ContentType)
        .Produces(BadRequest, typeof(ProblemDetails), ContentType)
        .WithName("Get All Products")
        .WithDescription("Get All Products")
        .WithSummary("Get All Products")
        .WithDisplayName("Get All Products")
        .WithTags(TagName)
        .WithOpenApi();

        app.MapGet($"/{Controller}/{{productId:Guid}}", async (IProductUseCases productUseCases, Guid productId, CancellationToken cancellation) =>
        {
            return await GetById(productUseCases, productId, cancellation);
        })
        .Produces(Success, typeof(Product), ContentType)
        .Produces(BadRequest, typeof(ProblemDetails), ContentType)
        .WithName("Get Product By Id")
        .WithDescription("Get Product By Id")
        .WithSummary("Get Product By Id")
        .WithDisplayName("Get Product By Id")
        .WithTags(TagName)
        .WithOpenApi();

        app.MapGet($"/{Controller}/by-name/{{productName}}", async (IProductUseCases productUseCases, string productName, CancellationToken cancellation) =>
        {
            return await GetByName(productUseCases, productName, cancellation);
        })
        .Produces(Success, typeof(Product), ContentType)
        .Produces(BadRequest, typeof(ProblemDetails), ContentType)
        .WithName("Get Product By Name")
        .WithDescription("Get Product By Name")
        .WithSummary("Get Product By Name")
        .WithDisplayName("Get Product By Name")
        .WithTags(TagName)
        .WithOpenApi();

        app.MapPost($"/{Controller}", async (IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation) =>
        {
            return await CreateProduct(productUseCases, product, cancellation);
        })
        .Accepts(typeof(CreateProductInput), ContentType)
        .Produces(Success, typeof(Product), ContentType)
        .Produces(BadRequest, typeof(ProblemDetails), ContentType)
        .WithName("Create Product")
        .WithDescription("Create Product")
        .WithSummary("Create Product")
        .WithDisplayName("Create Product")
        .WithTags(TagName)
        .WithOpenApi();

        return app;
    }

    public static async Task<IResult> GetAllProducts(IProductUseCases productUseCases, CancellationToken cancellation)
    {
        var products = await productUseCases.GetProducts(cancellation);
        return products.Match(x =>
                    Results.Ok(x),
                    x => Results.Problem(
                        type: HttpStatusCode.BadRequest.ToString(),
                        title: "Error",
                        detail: x.ToSeq().Head.Message,
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
    }

    public static async Task<IResult> GetById(IProductUseCases productUseCases, Guid productId, CancellationToken cancellation)
    {
        const string errorMessage = "Error while getting product by id.";

        var result = await productUseCases.GetProductById(productId, cancellation);

        return result.Match(x =>
            Results.Ok(x),
            x => Results.Problem(
                type: HttpStatusCode.BadRequest.ToString(),
                title: errorMessage,
                detail: x.ToSeq().Head.Message,
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    public static async Task<IResult> GetByName(IProductUseCases productUseCases, string productName, CancellationToken cancellation)
    {
        const string errorMessage = "Error while getting product by name.";

        var result = await productUseCases.GetProductByName(productName, cancellation);

        return result.Match(x =>
            Results.Ok(x),
            x => Results.Problem(
                type: HttpStatusCode.BadRequest.ToString(),
                title: errorMessage,
                detail: x.ToSeq().Head.Message,
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    public static async Task<IResult> CreateProduct(IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation)
    {
        const string errorMessage = "Error while creating new product.";

        var result = await productUseCases.CreateProduct(product, cancellation);

        return result.Match(x =>
            Results.Ok(x),
            x => Results.Problem(
                type: HttpStatusCode.BadRequest.ToString(),
                title: errorMessage,
                detail: x.ToSeq().Head.Message,
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }
}
