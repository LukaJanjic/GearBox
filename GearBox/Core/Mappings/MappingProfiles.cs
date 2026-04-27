using AutoMapper;
using Core.Domain.Entities;
using Core.DTOs;

namespace Core.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Brand, BrandDto>();

        CreateMap<Category, CategoryDto>();

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand!.Name))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category!.Name));
    }
}
