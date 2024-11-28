using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Presentation.gRPC2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace CleanArchitectureSampleProject.Presentation.Web;

public class ProductgRPCClient(HttpClient httpClient)
{
    public async Task PostProductAsync(CancellationToken cancellationToken = default)
    {

    }

    public async Task<ProductBlazor[]> GetProductsAsync(int maxItems = 10000, CancellationToken cancellationToken = default)
    {
        //return await Task.FromResult(Enumerable.Empty<ProductBlazor>().ToArray());

        List<ProductBlazor>? products = Enumerable.Empty<ProductBlazor>().ToList();

        try
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7065");

            // Cria o cliente a partir da classe gerada
            var client = new Producter.ProducterClient(channel);

            // Faz uma chamada ao servidor
            var product = await client.SayHelloAsync(new HelloRequest { Name = "C#" });

            // Exibe a resposta
            Console.WriteLine($"Server replied: {product}");

            products.Add(new ProductBlazor
            {
                Id = new Guid(product.Id),
                Name = product.Name,
                Description = product.Description,
                CreationDate = product.CreationDate.ToDateTime(),
                Quantity = product.Quantity,
                Value = product.Value.ToDecimal(),
                Category = new CategoryBlazor
                {
                    Id = new Guid(product.Category.First().Id),
                    CategoryName = product.Category.First().CategoryName,
                    CreationDate = product.Category.First().CreationDate.ToDateTime()
                }
            });

            //foreach (var product in reply.Products)
            //{
            //    products.Add(new ProductBlazor
            //    {
            //        Id = new Guid(product.Id),
            //        Name = product.Name,
            //        Description = product.Description,
            //        CreationDate = product.CreationDate.ToDateTime(),
            //        Quantity = product.Quantity,
            //        Value = (product.Value.Value / (decimal)Math.Pow(100, product.Value.Scale)),
            //        Category = new CategoryBlazor()
            //    });
            //}
        }
        catch (Exception ex)
        {

        }
        return products?.ToArray() ?? [];
    }
}