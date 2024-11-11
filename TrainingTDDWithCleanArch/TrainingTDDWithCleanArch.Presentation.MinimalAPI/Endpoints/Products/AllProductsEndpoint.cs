using System.Collections.Frozen;
using System.Net;
using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;

namespace TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Products;

public static class AllProductsEndpoint
{
    public static WebApplication MapProducts(this WebApplication app)
    {
        app.MapGet("/products", async (IProductUseCases productUseCases, CancellationToken cancellation) => 
        { 
            return await Get(productUseCases, cancellation); 
        })
        .Produces(StatusCodes.Status200OK, typeof(FrozenSet<Product>), "application/json")
        .WithName("Products")
        .WithDescription("Products")
        .WithSummary("Summary goes here")
        .WithDisplayName("Products")
        .WithTags("Products")
        .WithOpenApi();
        return app;
    }

    public static async Task<IResult> Get(IProductUseCases productUseCases, CancellationToken cancellation)
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
}
