using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Core.Application.Outputs.Sells;

public sealed class CreateSellOutput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }
    public FrozenSet<CreateSellItemOutput> Items { get; set; }

    public CreateSellOutput()
    {

    }


    public static implicit operator CreateSellOutput(Sell sell)
    {
        return new CreateSellOutput
        {
            Id = sell.Id,
            Description = sell.Description,
            TotalValue = sell.TotalValue,
            CreationDate = sell.CreationDate,
            Items = FrozenSet<CreateSellItemOutput>.Empty,
        };
    }
}

public sealed class CreateSellItemOutput
{

}
