using System;
using Core.Domain.Entities;

namespace Core.Interfaces.Repositories;

public interface IProductsRepository
{
    List<Product> GetProducts();
    Product GetProductById(int id);
}
