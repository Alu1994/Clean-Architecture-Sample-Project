using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Products;

public class CreateCategoryInput
{
    public string? Name { get; set; }

    public CreateCategoryInput()
    {

    }

    public void SetCategory(Category category)
    {
        Name = category.Name;
    }

    public Category ToCategory()
    {
        return Category.MapToCategory(Guid.Empty, Name);
    }
}

public sealed class CreateCategoryInputValidator : AbstractValidator<CreateCategoryInput>
{
    public CreateCategoryInputValidator()
    {
        RuleFor(x => x.ToCategory()).SetValidator(new CategoryValidator());
    }
}