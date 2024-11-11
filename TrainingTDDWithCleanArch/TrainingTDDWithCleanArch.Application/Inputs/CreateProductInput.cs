using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;

namespace TrainingTDDWithCleanArch.Application.Inputs;

public sealed class CreateProductInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public CreateCategoryInput Category { get; set; }

    public CreateProductInput()
    {

    }

    public void SetCategory(Guid id)
    {
        if(Category is null) Category = new CreateCategoryInput();
        Category.SetCategoryId(id);
    }

    public Validation<Error, Product> ToProduct()
    {
        var category = Category.ToCategory();

        if (category.IsFail)
            return (Seq<Error>)category;

        return Product.Create(Name, Description, Value, Quantity, category.SuccessToArray().First());
    }
}
