using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;

public sealed class SellValidator : AbstractValidator<Sell>
{
    public SellValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.TotalValue).NotEmpty().GreaterThanOrEqualTo(0);
    }
}
