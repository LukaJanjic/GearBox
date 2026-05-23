using Core.Domain.Entities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class SeedDataService
{
    private static readonly Dictionary<string, string> ImageUrlMap = new()
    {
        ["Naturehike Cloud Up 2 šator"]         = "https://loremflickr.com/400/300/tent,camping?lock=1",
        ["Osprey Atmos 65 ranac"]               = "https://loremflickr.com/400/300/backpack,hiking?lock=2",
        ["The North Face Venture 2 jakna"]      = "https://loremflickr.com/400/300/rain,jacket?lock=3",
        ["Salomon Speedcross 6"]                = "https://loremflickr.com/400/300/trail,shoes?lock=4",
        ["Garmin Edge 540 bike computer"]       = "https://loremflickr.com/400/300/cycling,gps?lock=5",
        ["Naturehike Sleeping Bag"]             = "https://loremflickr.com/400/300/sleeping,bag?lock=6",
        ["The North Face Mountain Light Parka"] = "https://loremflickr.com/400/300/parka,winter?lock=7",
        ["Osprey Talon 44 ranac"]               = "https://loremflickr.com/400/300/backpack,trail?lock=8",
        ["Salomon XA Pro 3D v9"]                = "https://loremflickr.com/400/300/hiking,shoes?lock=9",
        ["Garmin fēnix 7 sat"]                 = "https://loremflickr.com/400/300/smartwatch,sport?lock=10",
        ["Naturehike camping kamp kuvar"]       = "https://loremflickr.com/400/300/camping,stove?lock=11",
        ["The North Face Borealis ranac"]       = "https://loremflickr.com/400/300/backpack,laptop?lock=12",
        ["Salomon Quest 4D Pro"]                = "https://loremflickr.com/400/300/hiking,boots?lock=13",
        ["Osprey Exos 48 ranac"]                = "https://loremflickr.com/400/300/backpack,mountain?lock=14",
        ["Garmin Forerunner 965"]               = "https://loremflickr.com/400/300/running,watch?lock=15",
        ["The North Face Thunder Roundie jakna"]= "https://loremflickr.com/400/300/jacket,outdoor?lock=16",
        ["Naturehike Pro šator 2-osoba"]        = "https://loremflickr.com/400/300/tent,outdoor?lock=17",
        ["Salomon Shoes Sense Ride 5"]          = "https://loremflickr.com/400/300/running,shoes?lock=18",
        ["Osprey Transporter roll-top"]         = "https://loremflickr.com/400/300/travel,bag?lock=19",
        ["The North Face Ultra Low Pro"]        = "https://loremflickr.com/400/300/trail,running?lock=20",
    };

    public static async Task SeedAsync(GearBoxContext context)
    {
        await PatchImageUrlsAsync(context);

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

    private static async Task PatchImageUrlsAsync(GearBoxContext context)
    {
        var stale = await context.Products
            .Where(p => p.PictureUrl != null &&
                       (p.PictureUrl.Contains("placeholder") || p.PictureUrl.Contains("picsum")))
            .ToListAsync();

        if (stale.Count == 0) return;

        foreach (var product in stale)
            if (product.Name != null && ImageUrlMap.TryGetValue(product.Name, out var url))
                product.PictureUrl = url;

        await context.SaveChangesAsync();
    }
}

public class SeedData
{
    public List<Brand> Brands { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Product> Products { get; set; } = new();
}
