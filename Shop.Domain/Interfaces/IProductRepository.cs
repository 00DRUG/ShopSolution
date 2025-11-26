using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task SaveChangesAsync();
    }
}
