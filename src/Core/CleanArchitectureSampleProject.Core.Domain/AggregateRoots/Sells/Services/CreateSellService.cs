using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Services;

public interface ICreateSellService
{
    Task<Results<Sell, ErrorList>> Execute(Sell sell, CancellationToken cancellationToken);
}

public sealed class CreateSellService : ICreateSellService
{
    private readonly ISellRepository _sellRepository;
    private readonly IValidator<Sell> _validator;

    public CreateSellService(ISellRepository sellRepository, IValidator<Sell> validator)
    {
        _sellRepository = sellRepository;
        _validator = validator;
    }

    public async Task<Results<Sell, ErrorList>> Execute(Sell sell, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(sell);
        if (validationResult.IsValid is false) return new ErrorList(validationResult);
                
        //sell.Create();  // Send Update Stock Event
        var creationResult = await _sellRepository.Insert(sell, cancellationToken);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);

        return sell;
    }
}
