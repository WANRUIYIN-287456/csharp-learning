using Exercise13.Models;

namespace Exercise13.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync (int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}