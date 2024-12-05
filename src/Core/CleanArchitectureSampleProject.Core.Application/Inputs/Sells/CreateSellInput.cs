using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class CreateSellInput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public List<CreateSellItemInput> Items { get; set; }

    public Sell ToSell(ICollection<SellItem>? sellItems = null)
    {
        var sell = Sell.MapToSell(Description);
        sellItems ??= ToNewSellItems(Items, sell.Id);
        sell.SetItems(sellItems!);
        return sell;
    }

    public static List<SellItem> ToNewSellItems(List<CreateSellItemInput>? items, Guid id)
    {
        if (items is null or { Count: 0 }) return [];

        List<SellItem> list = [];
        foreach (var item in items)
        {
            list.Add(new CreateSellItemInput(id, item));
        }
        return list;
    }
}

public sealed class CreateSellValidator : AbstractValidator<CreateSellInput>
{
    public CreateSellValidator()
    {
        RuleFor(sell => sell.ToSell(null)).SetValidator(new SellValidator());
    }
}