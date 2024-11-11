using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;

namespace TrainingTDDWithCleanArch.Application.Inputs;

public sealed class CreateCategoryInput
{
    public Guid? Id { get; set; }
    public string? CategoryName { get; set; }

    public CreateCategoryInput()
    {

    }

    public void SetCategoryId(Guid id)
    {
        Id = id;
    }

    public Validation<Error, Category> ToCategory()
    {
        if(Id is null)
            return Category.CreateNew(CategoryName);
        return Category.Create(Id, "CategoryName");
    }
}