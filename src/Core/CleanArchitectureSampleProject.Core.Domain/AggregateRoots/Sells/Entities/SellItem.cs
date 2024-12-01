using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

public sealed class SellItem
{
    public Guid Id { get; set; }
    public Guid SellId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreationDate { get; set; }

    // ==== Navigation Properties ====
    public Sell Sell { get; set; }
    public Product Product { get; set; }
    // ==== Navigation Properties ====
}
