using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Validators;

public sealed class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public interface IGetOrCreateCategoryValidator : IValidator<Category>
{

}

public sealed class GetOrCreateCategoryValidator : AbstractValidator<Category>, IGetOrCreateCategoryValidator
{
    public GetOrCreateCategoryValidator()
    {
        When(x => string.IsNullOrWhiteSpace(x.Name) is false, () => { })
        .Otherwise(() => 
        {
            When(x => x.Id != null && x.Id != Guid.Empty, () => { })
            .Otherwise(() =>
            {
                RuleFor(x => x.Id).NotEmpty().WithName("Id or Name").WithMessage("'Category' 'Id' or 'Name' must be informed.");
            });
        });
    }
}