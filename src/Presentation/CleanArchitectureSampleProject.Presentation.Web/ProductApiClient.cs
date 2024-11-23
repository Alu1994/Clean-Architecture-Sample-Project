using CleanArchitectureSampleProject.Application.Inputs;

namespace CleanArchitectureSampleProject.Presentation.Web;

public class ProductApiClient(HttpClient httpClient)
{
    public async Task PostProductAsync(CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        await Task.Delay(500, cancellationToken);

        int rand = new Random().Next();
        await httpClient.PostAsJsonAsync("/product", new CreateProductInput
        {
            Name = $"Meu Produto Name {rand}",
            Description = $"Meu Produto Description{rand}",
            Quantity = 10,
            Value = 20,
            Category = new CategoryInput
            {
                CategoryName = $"CategoryName{rand}"
            }
        }, cancellationToken);

        await Task.Yield();
    }

    public async Task<ProductBlazor[]> GetProductsAsync(int maxItems = 10000, CancellationToken cancellationToken = default)
    {
        List<ProductBlazor>? products = null;

        try
        {
            await foreach (var product in httpClient.GetFromJsonAsAsyncEnumerable<ProductBlazor>("/product", cancellationToken))
            {
                if (products?.Count >= maxItems)
                {
                    break;
                }
                if (product is not null)
                {
                    products ??= [];
                    products.AddRange(product);
                }
            }
        }
        catch (Exception ex)
        {

        }
        return products?.ToArray() ?? [];
    }
}

public class ProductBlazor
{
    public ProductBlazor()
    {

    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public CategoryBlazor Category { get; set; }
}

public class CategoryBlazor
{
    public CategoryBlazor()
    {

    }

    public Guid Id { get; set; }
    public string CategoryName { get; set; }
    public DateTime CreationDate { get; set; }
}