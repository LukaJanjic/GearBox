using Core.Domain.Entities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController(ICartService cartService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCart()
    {
        var cart = await cartService.GetCartAsync(UserId);
        return Ok(cart ?? new ShoppingCart { UserId = UserId });
    }

    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCart>> AddItem(CartItem item)
    {
        var cart = await cartService.AddItemAsync(UserId, item);
        return Ok(cart);
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<ShoppingCart>> UpdateQuantity(int productId, [FromBody] int quantity)
    {
        var cart = await cartService.UpdateQuantityAsync(UserId, productId, quantity);
        return Ok(cart);
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<ShoppingCart>> RemoveItem(int productId)
    {
        var cart = await cartService.RemoveItemAsync(UserId, productId);
        return Ok(cart);
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        await cartService.ClearCartAsync(UserId);
        return NoContent();
    }
}
