using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs;

public class CategoryInput
{
    public Guid? Id { get; set; }
    public string? CategoryName { get; set; }

    public CategoryInput()
    {

    }

    public void SetCategory(Category category)
    {
        Id = category.Id;
        CategoryName = category.Name;
    }

    public Category ToCategory()
    {
        return Category.MapToCategory(Id, CategoryName);
    }
}

public sealed class UpdateCategoryInput : CategoryInput;

public sealed class CreateCategoryValidator : AbstractValidator<CategoryInput>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.ToCategory()).SetValidator(new CategoryValidator());
    }
}

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryInput>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ToCategory()).SetValidator(new CategoryValidator());
    }
}