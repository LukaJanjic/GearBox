using System.Text;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Mappings;
using Core.Services;
using Infrastructure.Cart;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors(opt => opt.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "http://localhost:3000");
        }));

        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfiles>());
        services.AddControllers();

        services.AddDbContext<GearBoxContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IProductsService,  ProductsService>();
        services.AddScoped<IBrandService,     BrandsService>();
        services.AddScoped<ICategoryService,  CategoriesService>();
        services.AddScoped<IAuthService,      AuthService>();

        // Identity
        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength         = 6;
            opt.Password.RequireUppercase       = false;
            opt.Password.RequireLowercase       = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<GearBoxContext>();

        // JWT Authentication
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = key,
                    ValidateIssuer           = false,
                    ValidateAudience         = false,
                    ValidateLifetime         = true,
                };
            });

        services.AddAuthorization();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(config.GetConnectionString("Redis") ?? "localhost:6379"));
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}
