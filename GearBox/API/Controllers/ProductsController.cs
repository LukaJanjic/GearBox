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
        public ActionResult GetProducts()
        {
            return Ok(service.GetProducts());
        }

        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            var product = service.GetProductById(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            return Ok(product);
        }


    }
}
