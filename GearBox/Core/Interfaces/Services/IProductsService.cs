using System;
using Core.Domain.Entities;

namespace Core.Interfaces.Services;

public interface IProductsService
{
    List<Product> GetProducts();
    Product GetProductById(int id);
}
