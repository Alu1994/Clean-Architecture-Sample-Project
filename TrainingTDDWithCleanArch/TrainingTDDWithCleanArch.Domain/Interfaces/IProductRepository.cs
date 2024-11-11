using System.Collections.Frozen;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;

namespace TrainingTDDWithCleanArch.Domain.Interfaces;

public interface IProductRepository
{
    Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation);
    Task<Validation<Error, Product>> GetById(Guid id, CancellationToken cancellation);
    Task<Validation<Error, Product>> GetByName(string productName, CancellationToken cancellation);
    Task<ValidationResult> Insert(Product product, CancellationToken cancellation);
    Task<ValidationResult> Update(Product product, CancellationToken cancellation);
}