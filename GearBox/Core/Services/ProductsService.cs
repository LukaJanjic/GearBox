using AutoMapper;
using Core.Domain.Entities;
using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.RequestHelpers;
using Core.Specifications.Products;

namespace Core.Services;

public class ProductsService(IGenericRepository<Product> repo, IMapper mapper) : IProductsService
{
    public async Task<Pagination<ProductDto>> GetProductsAsync(ProductQueryParams queryParams)
    {
        var spec = new ProductsWithBrandAndCategorySpecification(queryParams);
        var products = await repo.GetAllAsync(spec);
        var totalCount = await repo.CountAsync(spec);

        return new Pagination<ProductDto>
        {
            PageIndex = queryParams.PageIndex,
            PageSize = queryParams.PageSize,
            TotalCount = totalCount,
            Data = mapper.Map<List<ProductDto>>(products)
        };
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var spec = new ProductsWithBrandAndCategorySpecification(id);
        var product = await repo.GetEntityAsync(spec)
            ?? throw new NotFoundException($"Product with id {id} not found");
        return mapper.Map<ProductDto>(product);
    }
}
