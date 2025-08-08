using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No product found");

            // convert data from entity to dto
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list.Any() ? Ok(list) : NotFound("No product found");
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            // get single data
            var productData = await productInterface.GetByIdAsync(id);
            if (productData == null)
                return NotFound("Product not found");
            // convert data from entity to dto 
            var (_product, _) = ProductConversion.FromEntity(productData, null);
            return _product is not null ? Ok(_product) : NotFound("Product not found");
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDto product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert dto to entity
            var productEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(productEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDto product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert dto to entity
            var productEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(productEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDto product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var productEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(productEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
