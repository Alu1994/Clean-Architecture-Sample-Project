using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Core.Application.Outputs.Products;

public sealed class GetProductOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public CategoryOutput Category { get; set; }
    public DateTime CreationDate { get; set; }

    public GetProductOutput()
    {

    }

    public static implicit operator GetProductOutput(Product product)
    {
        return new GetProductOutput
        {
            Id = product.Id,
            Name = product.Name,
            CreationDate = product.CreationDate,
            Quantity = product.Quantity,
            Category = product.Category,
            Description = product.Description,
            Value = product.Value
        };
    }
}
