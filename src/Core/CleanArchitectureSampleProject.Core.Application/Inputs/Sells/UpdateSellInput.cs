using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class UpdateSellInput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public List<UpdateSellItemInput> Items { get; set; }

    public Sell ToSell(ICollection<SellItem>? sellItems = null)
    {
        var sell = Sell.MapToSell(Description, Id);
        sellItems ??= ToNewSellItems(Items, Id);
        sell.SetItems(sellItems!);
        return sell;
    }

    public static List<SellItem> ToNewSellItems(List<UpdateSellItemInput>? items, Guid id)
    {
        if (items is null or { Count: 0 }) return [];

        List<SellItem> list = [];
        foreach (var item in items)
        {
            list.Add(new UpdateSellItemInput(id, item));
        }
        return list;
    }
}

public sealed class UpdateSellValidator : AbstractValidator<UpdateSellInput>
{
    public UpdateSellValidator()
    {
        RuleFor(sell => sell.Id).NotEmpty();
        RuleFor(sell => sell.ToSell(null)).SetValidator(new SellValidator());
    }
}