using System.Collections.Frozen;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;

namespace TrainingTDDWithCleanArch.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Validation<Error, FrozenSet<Category>>> Get(CancellationToken cancellation);
    Task<Validation<Error, Category>> GetById(Guid id, CancellationToken cancellation);
    Task<Validation<Error, Category>> GetByName(string categoryName, CancellationToken cancellation);
    Task<ValidationResult> Insert(Category category, CancellationToken cancellation);
    Task<ValidationResult> Update(Category category, CancellationToken cancellation);
}