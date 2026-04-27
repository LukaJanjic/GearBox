using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Mappings;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfiles>());
        services.AddControllers();
        services.AddDbContext<GearBoxContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IBrandService, BrandsService>();
        return services;
    }
}
