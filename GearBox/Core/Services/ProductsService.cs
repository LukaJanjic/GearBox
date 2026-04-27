using System;
using Core.Domain.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Core.Services;

public class ProductsService(IProductsRepository repo) : IProductsService
{
    public Product GetProductById(int id)
    {
        return repo.GetProductById(id);
    }

    public List<Product> GetProducts()
    {
        return repo.GetProducts();
    }
}

