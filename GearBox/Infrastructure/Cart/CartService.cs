using Core.Domain.Entities;
using Core.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Cart;

public class CartService(IConnectionMultiplexer redis) : ICartService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private static readonly TimeSpan Expiry = TimeSpan.FromDays(30);
    private static string Key(string userId) => $"cart:{userId}";

    public async Task<ShoppingCart?> GetCartAsync(string userId)
    {
        var data = await _db.StringGetAsync(Key(userId));
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>((string)data!);
    }

    public async Task<ShoppingCart> AddItemAsync(string userId, CartItem item)
    {
        var cart = await GetCartAsync(userId) ?? new ShoppingCart { UserId = userId };

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing != null)
            existing.Quantity += item.Quantity;
        else
            cart.Items.Add(item);

        await SaveAsync(userId, cart);
        return cart;
    }

    public async Task<ShoppingCart> RemoveItemAsync(string userId, int productId)
    {
        var cart = await GetCartAsync(userId) ?? new ShoppingCart { UserId = userId };
        cart.Items.RemoveAll(i => i.ProductId == productId);
        await SaveAsync(userId, cart);
        return cart;
    }

    public async Task<ShoppingCart> UpdateQuantityAsync(string userId, int productId, int quantity)
    {
        var cart = await GetCartAsync(userId) ?? new ShoppingCart { UserId = userId };
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = quantity;
        }

        await SaveAsync(userId, cart);
        return cart;
    }

    public async Task<bool> ClearCartAsync(string userId)
    {
        return await _db.KeyDeleteAsync(Key(userId));
    }

    private async Task SaveAsync(string userId, ShoppingCart cart)
    {
        var json = JsonSerializer.Serialize(cart);
        await _db.StringSetAsync(Key(userId), json, Expiry);
    }
}
