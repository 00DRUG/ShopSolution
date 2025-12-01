using Shop.Application.DTOs;
using Shop.Domain;
using Shop.Domain.Interfaces;
using static Shop.Application.Services.IProductService;

namespace Shop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            //Entity -> Dto
            return products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.ImgUrl,
                p.Price,
                p.Description,
                p.StockQuantity));
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return null;
            return new ProductDto(
                product.Id,
                product.Name,
                product.ImgUrl,
                product.Price,
                product.Description,
                product.StockQuantity);
        }
        public async Task<int> CreateAsync(CreateProductDto dto)
        {
            var product = new Product(dto.Name, dto.ImgUrl);

            //Optional price field for extra info
            if (dto.Price.HasValue || !string.IsNullOrEmpty(dto.Description))
            {
                product.UpdateDetails(dto.Price ?? 0, dto.Description);
            }
            
            //save to db
            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return product.Id;

        }
        public async Task UpdateStockAsync(int id, int newQuantity)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new Exception($"Product with id {id} not found.");
            product.UpdateStock(newQuantity);
            await _repository.SaveChangesAsync();
        }
        public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize);
            var dtoItems = items.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.ImgUrl,
                p.Price,
                p.Description,
                p.StockQuantity));
            return new PagedResult<ProductDto>(dtoItems, totalCount, page, pageSize);
        }
    }
}
