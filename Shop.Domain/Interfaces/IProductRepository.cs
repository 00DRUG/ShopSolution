namespace Shop.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task SaveChangesAsync();
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    }
}
