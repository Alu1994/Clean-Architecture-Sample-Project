using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;

namespace CleanArchitectureSampleProject.Core.Application.Outputs.Sells;

public sealed class UpdateSellOutput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }
    public List<UpdateSellItemOutput> Items { get; set; }

    public UpdateSellOutput()
    {

    }


    public static implicit operator UpdateSellOutput(Sell sell)
    {
        return new UpdateSellOutput
        {
            Id = sell.Id,
            Description = sell.Description,
            TotalValue = sell.TotalValue,
            CreationDate = sell.CreationDate,
            Items = Enumerable.Empty<UpdateSellItemOutput>().ToList(),
        };
    }
}

public sealed class UpdateSellItemOutput
{

}
