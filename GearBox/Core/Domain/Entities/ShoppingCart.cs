namespace Core.Domain.Entities;

public class ShoppingCart
{
    public string UserId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    public int ItemCount => Items.Sum(i => i.Quantity);
}
