using AutoMapper;
using Core.Domain.Entities;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.Products;

namespace Core.Services;

public class ProductsService(IGenericRepository<Product> repo, IMapper mapper) : IProductsService
{
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        var spec = new ProductsWithBrandAndCategorySpecification();
        var products = await repo.GetAllAsync(spec);
        return mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var spec = new ProductsWithBrandAndCategorySpecification(id);
        var product = await repo.GetEntityAsync(spec);
        return mapper.Map<ProductDto?>(product);
    }
}
