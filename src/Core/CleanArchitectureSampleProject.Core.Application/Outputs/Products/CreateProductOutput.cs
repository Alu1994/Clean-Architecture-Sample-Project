﻿using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Core.Application.Outputs.Products;

public sealed class CreateProductOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public CategoryOutput Category { get; set; }
    public DateTime CreationDate { get; set; }

    public CreateProductOutput()
    {

    }

    public static implicit operator CreateProductOutput(Product product)
    {
        return new CreateProductOutput
        {
            Id = product.Id,
            Name = product.Name,
            CreationDate = product.CreationDate,
            Quantity = product.Quantity,
            Category = product.Category,
            Description = product.Description,
            Value = product.Value
        };
    }
}
