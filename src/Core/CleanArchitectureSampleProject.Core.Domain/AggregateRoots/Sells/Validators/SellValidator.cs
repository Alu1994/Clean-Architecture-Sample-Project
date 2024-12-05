namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;

public sealed class SellValidator : AbstractValidator<Sell>
{
    public SellValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new SellItemValidator());
    }
}
