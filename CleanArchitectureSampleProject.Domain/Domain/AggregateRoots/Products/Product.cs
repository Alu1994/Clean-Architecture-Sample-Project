﻿using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using LanguageExt.ClassInstances;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Products;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public Category Category { get; set; }

    // Foreign Key
    public Guid CategoryId { get; set; }

    public Product()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    private Product(Category category) : this()
    {
        ArgumentNullException.ThrowIfNull(category);
        SetCategory(category);
    }

    public static Validation<Error, Product> CreateExistent(Guid id, string name, string description, decimal? value, int? quantity, Category category, DateTime? creationDate = null)
    {
        return new Product(category)
        {
            Name = name,
            Description = description,
            Value = value.Value,
            Quantity = quantity.Value
        }
        .WithId(id)
        .WithCreationDate(creationDate);
    }

    private Product WithCreationDate(DateTime? creationDate)
    {
        if (creationDate is not null)
            CreationDate = creationDate.Value;
        return this;
    }

    public static Validation<Error, Product> CreateNew(string name, string description, decimal? value, int? quantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.New($"{nameof(Name)} must not be null.");

        if (string.IsNullOrWhiteSpace(description))
            return Error.New($"{nameof(Description)} must not be null.");

        if (value is null)
            return Error.New($"{nameof(Value)} must not be null.");

        if (quantity is null)
            return Error.New($"{nameof(Quantity)} must not be null.");

        if (category is null)
            return Error.New($"{nameof(Category)} must not be null.");

        return new Product(category)
        {
            Name = name,
            Description = description,
            Value = value.Value,
            Quantity = quantity.Value
        };
    }

    public void SetCategory(Category category)
    {
        Category = category;
        CategoryId = category.Id;
    }

    public ValidationResult ChangeCategory(Category category)
    {
        if (category is null)
        {
            return new ValidationResult($"{nameof(Category)} must not be null.");
        }
        SetCategory(category);
        return ValidationResult.Success!;
    }

    public ValidationResult UpdateValue(decimal? value)
    {
        if (value is null)
        {
            return new ValidationResult($"{nameof(Value)} must not be null.");
        }
        Value = value.Value;
        return ValidationResult.Success!;
    }

    public ValidationResult AddQuantity(int? quantity)
    {
        if (quantity is null)
        {
            return new ValidationResult($"{nameof(Quantity)} must not be null.");
        }
        Quantity += quantity.Value;
        return ValidationResult.Success!;
    }

    public ValidationResult SubtractQuantity(int? quantity)
    {
        if (quantity is null)
        {
            return new ValidationResult($"{nameof(Quantity)} must not be null.");
        }
        Quantity -= quantity.Value;
        return ValidationResult.Success!;
    }

    public Product WithCategory(Category category)
    {
        SetCategory(category);
        return this;
    }

    private Product WithId(Guid id)
    {
        Id = id;
        return this;
    }
}
