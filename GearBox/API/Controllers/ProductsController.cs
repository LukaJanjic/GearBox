using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductsService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            return Ok(await service.GetProductsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await service.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            return Ok(product);
        }


    }
}
