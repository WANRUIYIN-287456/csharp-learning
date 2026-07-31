using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Exercise11.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Exercise11.Models;

public partial class Order : IValidatableObject
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime OrderDate { get; set; }

    [ValidateNever]
    public virtual Customer Customer { get; set; } = null!;

    [ValidateNever]
    public virtual Product Product { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext))!;
        var product = context.Products.Find(ProductId);

        if (product != null && Quantity > product.Stock)
        {
            yield return new ValidationResult(
                $"Only {product.Stock} units of {product.Name} are in stock.",
                new[] { nameof(Quantity) }
            );
        }
    }
}
