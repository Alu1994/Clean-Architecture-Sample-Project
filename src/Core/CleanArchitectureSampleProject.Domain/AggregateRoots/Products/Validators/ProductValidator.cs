using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;

public sealed class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Value).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);

        When(x => x.CategoryId == Guid.Empty && (x.Category is null || string.IsNullOrWhiteSpace(x?.Category?.Name)), () =>
        {
            RuleFor(x => x).Null().WithName("Category").WithMessage("Category must be informed.");
        });
    }
}
