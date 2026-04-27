using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IProductsService
{
    Task<List<ProductDto>> GetProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
}
