using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf.Collections;

namespace CleanArchitectureSampleProject.Presentation.gRPC2.Services;

public class ProductService : Producter.ProducterBase
{
    private readonly ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
    }

    public override async Task<GetProductOutput> SayHello(HelloRequest request, ServerCallContext context)
    {
        var productGuid = Guid.NewGuid().ToString();
        var categoryGuid = Guid.NewGuid().ToString();
        var categories = new RepeatedField<CategoryOutput>
        {
            new CategoryOutput
            {
                Id = categoryGuid,
                CategoryName = $"Category-{request.Name}",
                CreationDate = Timestamp.FromDateTime(DateTime.UtcNow)
            }
        };

        var product = new GetProductOutput
        {
            Id = productGuid,
            Name = $"Name-{request.Name}",
            Description = $"Description-{request.Name}",
            Quantity = 10,
            CreationDate = Timestamp.FromDateTime(DateTime.UtcNow),
            Value = 100.425M.ToProtoDecimal(),
        };
        product.Category.AddRange(categories);

        return await Task.FromResult(product);
    }
}
