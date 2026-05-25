using Core.Domain.Entities;

namespace Core.Interfaces.Services;

public interface ICartService
{
    Task<ShoppingCart?> GetCartAsync(string userId);
    Task<ShoppingCart>  AddItemAsync(string userId, CartItem item);
    Task<ShoppingCart>  RemoveItemAsync(string userId, int productId);
    Task<ShoppingCart>  UpdateQuantityAsync(string userId, int productId, int quantity);
    Task<bool>          ClearCartAsync(string userId);
}
