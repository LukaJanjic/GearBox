using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IBrandService
{
    Task<List<BrandDto>> GetBrandsAsync();
    Task<BrandDto> GetBrandByIdAsync(int id);
}
