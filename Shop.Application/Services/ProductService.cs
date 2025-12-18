using AutoMapper;
using Shop.Application.Common;
using Shop.Application.DTOs;
using Shop.Domain;
using Shop.Domain.Interfaces;
using static Shop.Application.Services.IProductService;

namespace Shop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        private readonly AutoMapper.IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            //Entity -> Dto
            return _mapper.Map<IEnumerable<ProductDto>>(products);
            
            /*return products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.ImgUrl,
                p.Price,
                p.Description,
                p.StockQuantity));*/
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            return _mapper.Map<ProductDto>(product);
            
            /*if (product == null)
                return null;
            return new ProductDto(
                product.Id,
                product.Name,
                product.ImgUrl,
                product.Price,
                product.Description,
                product.StockQuantity);*/
        }
        public async Task<int> CreateAsync(CreateProductDto dto)
        {
            // without AutoMapper
            //var product = new Product(dto.Name, dto.ImgUrl);

            var product  = _mapper.Map<Product>(dto);

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

            var dtoItems = _mapper.Map<IEnumerable<ProductDto>>(items);

            /*var dtoItems = items.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.ImgUrl,
                p.Price,
                p.Description,
                p.StockQuantity));
            */

            return new PagedResult<ProductDto>(dtoItems, totalCount, page, pageSize);
        }
        public async Task<CursorResult<ProductDto>> GetCursorPagedAsync(int? lastId, int pageSize)
        {
            var products = await _repository.GetCursorPagedAsync(lastId, pageSize);
            var productsList = products.ToList();

            // Determine if there is a next page
            bool hasNextPage = productsList.Count > pageSize;

            // Delete the extra item if exists 
            var itemsToReturn = hasNextPage? productsList.Take(pageSize): productsList;
            
            var dtoItems = _mapper.Map<IEnumerable<ProductDto>>(itemsToReturn);
            
            /*var dtoItems = products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.ImgUrl,
                p.Price,
                p.Description,
                p.StockQuantity));
            */

            return new CursorResult<ProductDto>
            {
                Items = dtoItems,
                NextCursor = hasNextPage ? dtoItems.LastOrDefault()?.Id : null
            };
        }
    }
}
