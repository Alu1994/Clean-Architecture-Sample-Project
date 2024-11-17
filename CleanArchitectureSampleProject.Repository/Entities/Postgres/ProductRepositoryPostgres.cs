using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Repository.Entities.Postgres;

public sealed class ProductRepositoryPostgres(ProductDataContext context) : IProductRepository
{
    private readonly ProductDataContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<Validation<Error, FrozenSet<Product>>> Get(CancellationToken cancellation)
    {
        try
        {
            var products = await _context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            if (products == null)
            {
                return Enumerable.Empty<Product>().ToFrozenSet();
            }
            return products.ToFrozenSet();
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving all Products: {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Product>> GetById(Guid id, CancellationToken cancellation)
    {
        try
        {
            var product = await _context.Products.Include(x => x.Category).AsNoTracking().FirstAsync(x => x.Id == id);
            return product!;
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Product with id '{id}': {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Product>> GetByName(string productName, CancellationToken cancellation)
    {
        try
        {
            var product = await _context.Products.Include(x => x.Category).AsNoTracking().FirstAsync(x => x.Name == productName);
            return product!;
        }
        catch (Exception ex)
        {
            return Error.New($"Error while retrieving Product with name '{productName}': {ex.Message}", ex);
        }
    }

    public async Task<ValidationResult> Insert(Product product, CancellationToken cancellation)
    {
        try
        {
            product.Category = null;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Product '{product.Name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Update(Product product, CancellationToken cancellation)
    {
        try
        {
            product.Category = null;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Product '{product.Name}': {ex.Message}");
        }
    }
}
