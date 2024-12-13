using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;
using FluentValidation;
using System.Collections.ObjectModel;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class CreateSellInput
{
    public string Description { get; set; }
    public Collection<CreateSellItemInput> Items { get; set; }

    public Sell ToSell()
    {
        var sell = Sell.ToSell(Description);
        var sellItems = ToNewSellItems(sell.Id, Items);
        sell.SetItems(sellItems!);
        return sell;
    }

    public static Collection<SellItem> ToNewSellItems(Guid id, Collection<CreateSellItemInput>? items)
    {
        if (items is null or { Count: 0 }) return [];

        Collection<SellItem> sellItems = [];
        foreach (var item in items)
        {
            sellItems.Add(item.ToSellItem(id));
        }
        return sellItems;
    }
}

public sealed class CreateSellValidator : AbstractValidator<CreateSellInput>
{
    public CreateSellValidator()
    {
        RuleFor(sell => sell.ToSell()).SetValidator(new SellValidator());
    }
}