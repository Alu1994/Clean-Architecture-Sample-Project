using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class UpdateSellInput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public List<CreateSellItemInput> Items { get; set; }

    public Sell ToSell(ICollection<SellItem>? sellItems = null)
    {
        var sell = Sell.MapToSell(Description, TotalValue);
        sellItems ??= Items?.ToNewSellItems(sell.Id);
        sell.SetItems(sellItems!);
        return sell;
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