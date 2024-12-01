using CleanArchitectureSampleProject.Core.Application.Outputs.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using FluentValidation;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class CreateSellInput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public FrozenSet<CreateSellItemInput> Items { get; set; }

    public CreateSellInput()
    {

    }

    public Sell ToSell(ICollection<SellItem>? sellItems = null)
    {
        //sellItems ??= Items?.ToSellItems();
        return Sell.MapToSell(Description, TotalValue, sellItems);
    }
}

public sealed class CreateSellItemInput
{

}

public sealed class CreateSellValidator : AbstractValidator<CreateSellInput>
{
    public CreateSellValidator()
    {
        RuleFor(sell => sell.ToSell(null)).SetValidator(new SellValidator());
    }
}