using System.ComponentModel.DataAnnotations;
using Exercise11.Data;
using Exercise11.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Packaging.Signing;

namespace Exercise12.Tests;

public class UnitTest1
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Index_CalculateTotalStockValueCorrectly()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Products.Add(new Product { Name = "Product1", Price = 100.00m, Stock = 5 });
        context.Products.Add(new Product { Name = "Product2", Price = 50.00m, Stock = 2 });
        await context.SaveChangesAsync();

        var controller = new ProductsController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ProductListViewModel>(viewResult.Model);
        Assert.Equal(600, model.TotalStockValue);
    }

    [Theory]
    [InlineData(0.01, true)]        // minimum valid boundary
    [InlineData(100000, true)]      // maximum valid boundary
    [InlineData(0, false)]          // below minimum
    [InlineData(100001, false)]     // above maximum
    public void Product_PriceValidation_WorksCorrectly(decimal price, bool expectedValid)
    {
        var product = new Product { Name = "Test", Price = price, Stock = 5 };
        var context = new ValidationContext(product);
        var results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(product, context, results, validateAllProperties: true);

        Assert.Equal(expectedValid, isValid);
    }

    [Theory]
    [InlineData(0, true)]               // minimum valid boundary
    [InlineData(2147483647, true)]      // maximum valid boundary
    [InlineData(-1, false)]             // below minimum
    public void Product_StockValidation_WorksCorrectly(int stock, bool expectedValid)
    {
        var product = new Product { Name = "Test", Price = 10.00m, Stock = stock };
        var context = new ValidationContext(product);
        var results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(product, context, results, validateAllProperties: true);

        Assert.Equal(expectedValid, isValid);
    }

    [Fact]
    public void Order_Validation_FailsWhenQuantityExceedsStock()
    {
        var context = GetInMemoryContext();
        var product = new Product { Id = 1, Name = "Monitor", Price = 899.00m, Stock = 5 };
        context.Products.Add(product);
        context.SaveChanges();

        var order = new Order { ProductId = 1, Quantity = 100, CustomerId = 1 };

        // Since we need to get validation context from constructor, (refer Exercise11/Models/Order.cs)
        // it will return the real db context but we need AppDbContext for in-memory context.
        // Solution: register in-memory context so GetService can find it.
        var services = new ServiceCollection();
        services.AddSingleton(context);
        var serviceProvider = services.BuildServiceProvider();

        var validationContext = new ValidationContext(order, serviceProvider, items: null);
        var results = order.Validate(validationContext).ToList();

        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Quantity"));

    }

    [Fact]
    public async Task Order_DecreaseProductStock_WorksCorrectly()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Products.Add(new Product { Name = "TestProduct", Price = 10.00m, Stock = 10 });
        context.Customers.Add(new Customer { Name = "TestCustomer" });
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder().Build();
        var controller = new OrdersController(context, config);

        // Act
        var result = await controller.Create(new Order { CustomerId = 1, ProductId = 1, Quantity = 3 });

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        var orders = await context.Orders.ToListAsync();
        var product = await context.Products.FirstAsync(p => p.Id == 1);

        Assert.NotEmpty(orders);
        Assert.Equal(7, product.Stock);
    }

    [Fact]
    public async Task Order_Validation_InsufficientStock_WorksCorrectly()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Products.Add(new Product { Name = "TestProduct", Price = 10.00m, Stock = 10 });
        context.Customers.Add(new Customer { Name = "TestCustomer" });
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder().Build();
        var controller = new OrdersController(context, config);

        // Act
        var result = await controller.Create(new Order { CustomerId = 1, ProductId = 1, Quantity = 50 });

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Order>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("Quantity"));

        var orders = await context.Orders.ToListAsync();
        var product = await context.Products.FirstAsync(p => p.Id == 1);

        Assert.Empty(orders);
        Assert.Equal(10, product.Stock);
    }
}

