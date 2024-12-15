using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Products;

public sealed class UpdateCategoryInput
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }

    public UpdateCategoryInput()
    {

    }

    public void SetCategory(Category category)
    {
        Id = category.Id;
        Name = category.Name;
    }

    public Category ToCategory()
    {
        return Category.MapToCategory(Id, Name);
    }
}

public sealed class UpdateCategoryInputValidator : AbstractValidator<UpdateCategoryInput>
{
    public UpdateCategoryInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ToCategory()).SetValidator(new CategoryValidator());
    }
}