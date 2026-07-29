namespace Exercise9.Models;

public class ProductListViewModel
{
    public required List<Product> Products { get; set; }
    public decimal TotalStockValue { get; set; }
}
