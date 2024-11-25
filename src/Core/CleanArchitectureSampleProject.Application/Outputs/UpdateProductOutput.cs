using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Core.Application.Outputs;

public sealed class UpdateProductOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public CategoryOutput Category { get; set; }
    public DateTime CreationDate { get; set; }

    public UpdateProductOutput()
    {

    }

    public static implicit operator UpdateProductOutput(Product product)
    {
        return new UpdateProductOutput
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
