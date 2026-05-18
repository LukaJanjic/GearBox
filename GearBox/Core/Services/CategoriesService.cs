using AutoMapper;
using Core.Domain.Entities;
using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.Categories;

namespace Core.Services;

public class CategoriesService(IGenericRepository<Category> repo, IMapper mapper) : ICategoryService
{
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var spec = new CategoriesSpecification();
        var categories = await repo.GetAllAsync(spec);
        return mapper.Map<List<CategoryDto>>(categories);
    }
}
