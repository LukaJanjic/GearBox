using Core.DTOs;

namespace Core.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
}
