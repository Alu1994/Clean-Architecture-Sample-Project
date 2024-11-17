﻿using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Repository.Entities.Postgres;

public sealed class CategoryRepositoryPostgres(ProductDataContext context) : ICategoryRepository
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation)
    {
        try
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories == null) {
                return Enumerable.Empty<Category>().ToFrozenSet();
            }
            return categories.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Categories: {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            var category = await _context.Categories.FirstAsync(x => x.Id == id);
            return category!;
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Category>> GetByName(string categoryName, CancellationToken cancellation)
    {
        try
        {
            var category = await _context.Categories.FirstAsync(x => x.Name == categoryName);
            return category!;
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Category with name '{categoryName}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(Category category, CancellationToken cancellation)
    {
        try
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Category '{category.Name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Category category, CancellationToken cancellation)
    {
        try
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Updating Category '{category.Name}': {ex.Message}");
        }
    }
}