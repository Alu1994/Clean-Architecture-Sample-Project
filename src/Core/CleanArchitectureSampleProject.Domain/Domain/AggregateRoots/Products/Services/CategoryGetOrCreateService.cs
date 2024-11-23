using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;

public interface ICategoryGetOrCreateService
{
    Task<Validation<Error, Category>> Execute(Category category, CancellationToken cancellationToken);
}

public sealed class CategoryGetOrCreateService : ICategoryGetOrCreateService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryGetOrCreateService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Validation<Error, Category>> Execute(Category categoryInput, CancellationToken cancellationToken)
    {
        if (categoryInput is null) return Error.New("Category must not be null.");

        Validation<Error, Category> categoryResult = categoryInput.ValidateGetOrCreate();
        if(categoryResult.IsFail) return categoryResult.ToError();

        if (categoryInput.Id != Guid.Empty)
        {
            var categoryGetResult = await _categoryRepository.GetById(categoryInput.Id);
            if(categoryGetResult.IsFail) return categoryGetResult.ToError();
            return categoryGetResult.ToSuccess();
        }

        var creationResult = await _categoryRepository.Insert(categoryInput, cancellationToken);
        if (creationResult != ValidationResult.Success) return Error.New(creationResult.ErrorMessage!);

        return categoryInput;
    }
}
