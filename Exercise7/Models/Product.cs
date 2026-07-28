namespace Exercise7.Models;
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public required string Name { get; set; }

    [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100000")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }
}

public static class ProductStore
{
    public static List<Product> Products = new List<Product>
    {
        new Product { Id = 1, Name = "Keyboard", Price = 150.00m, Stock = 20 },
        new Product { Id = 2, Name = "Mouse", Price = 45.50m, Stock = 0 },
        new Product { Id = 3, Name = "Monitor", Price = 899.00m, Stock = 5 },
    };
}