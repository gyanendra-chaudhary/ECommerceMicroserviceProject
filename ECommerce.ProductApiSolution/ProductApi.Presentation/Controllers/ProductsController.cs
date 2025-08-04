using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct product) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await product.GetAllAsync();
            if (!products.Any())
                return NotFound("No product found");
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list.Any() ? Ok(list) : NotFound("No product found");
        }
    }
}
