using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;
using FluentValidation;
using System.Collections.ObjectModel;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class UpdateSellInput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Collection<UpdateSellItemInput> Items { get; set; }

    public Sell ToSell()
    {
        var sell = Sell.ToSell(Description, Id);
        var sellItems = ToNewSellItems(Items, Id);
        sell.SetItems(sellItems!);
        return sell;
    }

    public static Collection<SellItem> ToNewSellItems(Collection<UpdateSellItemInput>? items, Guid id)
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

public sealed class UpdateSellValidator : AbstractValidator<UpdateSellInput>
{
    public UpdateSellValidator()
    {
        RuleFor(sell => sell.Id).NotEmpty();
        RuleFor(sell => sell.ToSell()).SetValidator(new SellValidator());
    }
}
