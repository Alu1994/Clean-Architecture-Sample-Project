﻿using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Application.Inputs;

public sealed class CreateProductInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public CategoryInput Category { get; set; }

    public CreateProductInput()
    {

    }

    public CreateProductInput(UpdateProductInput input)
    {
        Name = input.Name;
        Description = input.Description;
        Value = input.Value;
        Quantity = input.Quantity;
        Category = input.Category;
    }

    public void SetCategory(Category category)
    {
        if(Category is null) Category = new CategoryInput();
        Category.SetCategory(category);
    }

    public Validation<Error, Product> ToProduct()
    {
        var category = Category.ToCategory();

        if (category.IsFail)
            return (Seq<Error>)category;

        return Product.CreateNew(Name, Description, Value, Quantity, category.SuccessToArray().First());
    }
}
