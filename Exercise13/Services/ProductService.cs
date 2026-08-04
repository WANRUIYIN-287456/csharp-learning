using Exercise13.Models;
using Exercise13.Repositories;
using NuGet.Protocol.Core.Types;

namespace Exercise13.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductListViewModel> GetProductListAsync()
    {
        var products = await _repository.GetAllAsync();
        return new ProductListViewModel
        {
            Products = products,
            TotalStockValue = products.Sum(p => p.Price * p.Stock)
        };
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        await _repository.AddAsync(product);
        return product;
    }
}