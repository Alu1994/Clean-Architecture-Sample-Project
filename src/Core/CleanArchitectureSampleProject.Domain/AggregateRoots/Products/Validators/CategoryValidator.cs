using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Validators;

public sealed class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}