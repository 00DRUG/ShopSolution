using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Common;
using Shop.Application.DTOs;
using Shop.Application.Services;
using Shop.Infrastructure.BackgroundJobs;
using static Shop.Application.Services.IProductService;

namespace Shop.Api.Controllers
{
    [ApiController]
    [ApiVersion("2.0")] // V2 version 
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsV2Controller : ControllerBase
    {
        private readonly IProductService _service;

        private readonly IStockQueue _queue;

        public ProductsV2Controller(IProductService productService, IStockQueue stockQueue)
        {
            _service = productService;
            _queue = stockQueue;
        }

        // GET: api/v2/products/by-page?page=1
        [HttpGet("by-page")]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        //Get: api/v2/products/by-cursor?lastId=10
        [HttpGet("by-cursor")]
        public async Task<ActionResult<CursorResult<ProductDto>>> GetCursorPaged([FromQuery] int? lastId, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetCursorPagedAsync(lastId, pageSize);
            return Ok(result);
        }
        //Async PATCH: api/v2/products/5/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStockAsync(int id, [FromBody] UpdateStockDto dto)
        {
            var message = new StockUpdateMessage(id, dto.NewQuantity);

            //Push to queue
            await _queue.QueueBackgroundWorkItemAsync(message);

            //Return
            return Accepted();
        }
    }
}
