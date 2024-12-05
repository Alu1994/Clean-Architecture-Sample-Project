using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;

public sealed class SellItemValidator : AbstractValidator<SellItem>
{
    public SellItemValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.SellId).NotEmpty();
        RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0);
    }
}
