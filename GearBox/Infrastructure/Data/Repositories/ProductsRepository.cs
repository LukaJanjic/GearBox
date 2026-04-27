using System;
using Core.Domain.Entities;
using Core.Interfaces.Repositories;
namespace Infrastructure.Data.Repositories;

public class ProductsRepository(GearBoxContext context) : IProductsRepository
{
    public Product GetProductById(int id)
    {
        var product = context.Products.FirstOrDefault(p => p.Id == id) ?? throw new Exception("Product not found");
        return product;
    }

    public List<Product> GetProducts()
    {
        return context.Products.ToList();
    }
}
