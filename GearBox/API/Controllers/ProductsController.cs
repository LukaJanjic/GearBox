using Core.Interfaces.Services;
using Core.RequestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductsService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetProducts([FromQuery] ProductQueryParams queryParams)
        {
            return Ok(await service.GetProductsAsync(queryParams));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            return Ok(await service.GetProductByIdAsync(id));
        }


    }
}
