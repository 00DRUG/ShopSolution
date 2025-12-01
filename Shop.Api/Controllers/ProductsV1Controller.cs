using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs;
using Shop.Application.Services;

namespace Shop.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")] // V1 version 
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsV1Controller : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsV1Controller(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Product with Id{id} not found.");
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
        {
            var newProductId = await _productService.CreateAsync(createProductDto);
            return CreatedAtAction(nameof(GetById), new { id = newProductId }, null);
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            try
            {
                await _productService.UpdateStockAsync(id, dto.NewQuantity);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
