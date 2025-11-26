using Microsoft.EntityFrameworkCore;
using Shop.Domain;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Shop.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopDbContext _context;
        public ProductRepository (ShopDbContext context)
        {
            _context = context;
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }
    }
}
