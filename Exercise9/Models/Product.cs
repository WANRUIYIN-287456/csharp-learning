using System.ComponentModel.DataAnnotations;

namespace Exercise9.Models;

public partial class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100000")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}