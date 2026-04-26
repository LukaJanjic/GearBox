using Core.Domain.Entities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class SeedDataService
{
    public static async Task SeedAsync(GearBoxContext context)
    {
        if (await context.Products.AnyAsync()) return;

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed-data.json");
        var json = await File.ReadAllTextAsync(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<SeedData>(json, options);

        if (data == null) return;

        foreach (var brand in data.Brands)
        {
            brand.Id = 0;
            context.Brands.Add(brand);
        }
        await context.SaveChangesAsync();

        foreach (var category in data.Categories)
        {
            category.Id = 0;
            context.Categories.Add(category);
        }
        await context.SaveChangesAsync();

        foreach (var product in data.Products)
        {
            product.Id = 0;
            context.Products.Add(product);
        }
        await context.SaveChangesAsync();
    }
}

public class SeedData
{
    public List<Brand> Brands { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Product> Products { get; set; } = new();
}