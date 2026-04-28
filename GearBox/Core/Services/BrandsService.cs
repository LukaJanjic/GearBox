using AutoMapper;
using Core.Domain.Entities;
using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.Brands;

namespace Core.Services;

public class BrandsService(IGenericRepository<Brand> repo, IMapper mapper) : IBrandService
{
    public async Task<BrandDto> GetBrandByIdAsync(int id)
    {
        var spec = new BrandsSpecification(id);
        var brand = await repo.GetEntityAsync(spec)
            ?? throw new NotFoundException($"Brand with id {id} not found");
        return mapper.Map<BrandDto>(brand);
    }

    public async Task<List<BrandDto>> GetBrandsAsync()
    {
        var spec = new BrandsSpecification();
        var brands = await repo.GetAllAsync(spec);
        return mapper.Map<List<BrandDto>>(brands);
    }
}

