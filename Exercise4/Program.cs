using System;

class Program
{
    static void Main()
    {
        var products = new List<Product>
        {
            new Product("Keyboard", 150.00m, 20),
            new Product("Mouse", 45.50m, 0),
            new Product("Monitor", 899.00m, 5),
            new Product("Webcam", 120.00m, 8),
            new Product("USB Cable", 15.00m, 100),
            new Product("Headset", 250.00m, 0),
        };

        // PART A
        // 1. Get all products that are out of stock.
        var outOfStock = products.Where(n => n.Stock == 0);
        Product.PrintResult("1. Out of Stock", outOfStock);

        // 2. Get the names of all products priced above 100, sorted by price descending.
        var expPro = products.Where(n => n.Price > 100).OrderByDescending(n => n.Price).Select(n => n.Name);
        var expProResult = "2. Expensive Products: ";
        foreach (string n in expPro) expProResult += $"{n}. ";
        Console.WriteLine(expProResult);

        // 3. Calculate the total value of all stock (Price × Stock, summed across all products)
        // var totalValue = products.Select( n => n.Price * n.Stock).Aggregate( (acc, n) => acc + n );
        var totalValue = products.Select(n => n.Price * n.Stock).Sum();
        Console.WriteLine($"3. Total Value: {totalValue}");

        // 4. Find the most expensive product (return the whole Product, not just the price).
        // var mostExpPro = products.OrderByDescending( n => n.Price ).Take(1);  // This return sequence/collection IEnumerable<Product> rather than single instance Product. Needs .First() or .toList() later. Not suitable here.
        var mostExpPro = products.OrderByDescending(n => n.Price).First();
        Console.WriteLine($"4. Most Epensive Product: {mostExpPro.Name} - {mostExpPro.Stock} - {mostExpPro.Price}");

        // 5. does any product cost more than 500? (boolean result)
        var hasExpPro = products.Any(n => n.Price > 500);
        Console.WriteLine($"5. Has product cost more than 500? {hasExpPro}");

        // 6. Count how many products are in stock (Stock > 0)
        var count = products.Count(n => n.Stock > 0);
        Console.WriteLine($"6. Number of products in stock: {count}");


        // PART B - proving deferred execution
        // 1. With Deferred execution - New Item in collection
        List<Product> stockList = new List<Product>(products);   // creates a NEW list, copying the items over
        var lowStockQuery = stockList.Where(p => p.Stock < 10);  // not executed yet 
        stockList.Add(new Product("New Item", 99.00m, 2));
        var totalStock = "7. Total Stock: ";
        foreach (var p in lowStockQuery)
        {
            totalStock += $"{p.Name}. ";
        }
        Console.WriteLine(totalStock);

        // 2. With Deferred execution - New Item not in collection
        List<Product> productList = new List<Product>(products);
        var inStockQuery = productList.Where(p => p.Stock < 10).ToList();
        productList.Add(new Product("New Item", 99.00m, 2));
        var totalProducts = "8. Total Products: ";
        foreach (var p in inStockQuery)
        {
            totalProducts += $"{p.Name}. ";
        }
        Console.WriteLine(totalProducts);

    }
}

class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public Product(string name, decimal price, int stock)
    {
        Name = name;
        Price = price;
        Stock = stock;
    }

    // Re-evaluates .Last() every single loop iteration as they are deferred queries,
    // not concrete lists. Correct but very inefficiency.
    // public static void PrintResult(string name, IEnumerable<Product> products)
    // {
    //     string result = $"{name} : ";

    //     foreach (Product n in products)
    //     {
    //         string separator = "";
    //         if (n != products.Last()) separator = ", ";
    //         result += $"{n.Name}{separator}";
    //     }

    //     Console.WriteLine(result);
    // }

    public static void PrintResult(string name, IEnumerable<Product> products)
    {
        var list = products.ToList();   // execute once, lock in
        string result = $"{name} : ";

        for (int i = 0; i < list.Count; i++)
        {
            string separator = (i == list.Count - 1) ? "" : ", ";
            result += $"{list[i].Name}{separator}";
        }

        Console.WriteLine(result);
    }
}