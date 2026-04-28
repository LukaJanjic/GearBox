using Core.DTOs;
using Core.RequestHelpers;

namespace Core.Interfaces.Services;

public interface IProductsService
{
    Task<Pagination<ProductDto>> GetProductsAsync(ProductQueryParams queryParams);
    Task<ProductDto?> GetProductByIdAsync(int id);
}
