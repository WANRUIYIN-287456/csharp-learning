using Microsoft.EntityFrameworkCore;
using Exercise13.Data;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Exercise13.Models;

namespace Exercise13.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync() => await _context.Products.ToListAsync();
    public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);
    public async Task AddAsync (Product product)
    {
        _context.Products.Add(product);  // does not touch database, tells EF Core's in-memory change tracker to remember this product and generate an INSERT when SaveChanges() called.
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync (Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}