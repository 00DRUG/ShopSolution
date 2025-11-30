using Shop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateProductDto createProductDto);
        Task UpdateStockAsync(int Id, int newQuantity);
        public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
        Task <PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize);
    }
}
