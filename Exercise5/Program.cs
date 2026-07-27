using System;

class Program
{
    static void Main()
    {
        List<Customer> customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Acme Corp" },
            new Customer { Id = 2, Name = "Globex Inc" },
            new Customer { Id = 3, Name = "Initech" },
        };

        List<Order> orders = new List<Order>
        {
            new Order { Id = 100, CustomerId = 1, ProductName = "Keyboard", Amount = 150.00m },
            new Order { Id = 101, CustomerId = 2, ProductName = "Monitor", Amount = 899.00m },
            new Order { Id = 102, CustomerId = 1, ProductName = "Mouse", Amount = 45.50m },
            new Order { Id = 103, CustomerId = 2, ProductName = "Webcam", Amount = 120.00m },
            new Order { Id = 104, CustomerId = 1, ProductName = "Headset", Amount = 250.00m },
            new Order { Id = 105, CustomerId = 99, ProductName = "Unknown Item", Amount = 10.00m },  // CustomerId 99 doesn't exist!
        };

        // 1. Join
        // Should excludes order 105 because JOIN defaults uses INNER JOIN which 
        // includes those rows that exists in both collections.
        var joined = orders.Join(
            customers,
            order => order.CustomerId,
            customer => customer.Id,
            (order, customer) => new
            {
                OrderId = order.Id,
                CustomerName = customer.Name,
                order.ProductName,
                order.Amount
            }
        );
        Console.WriteLine("Q1. Join");
        foreach (var row in joined)
        {
            Console.WriteLine($"{row.OrderId}. {row.CustomerName} - {row.ProductName} - {row.Amount:C2}");
        }

        // 2. Group by
        Console.WriteLine("\nQ2. Group By");
        var grouped = joined.GroupBy(g => g.CustomerName);
        foreach (var group in grouped)
        {
            var total = group.Sum( n => n.Amount);
            Console.WriteLine($"{group.Key}: {total:C2}");
        }

        // 3. Dictionary
        Dictionary<int, string> dict = [];
        foreach (var n in customers)
        {
            dict[n.Id] = n.Name;
        }

        // To fix warning for non-nullable value
        if (dict.TryGetValue(99, out string? name))
        {
            Console.WriteLine($"\n3. Dictionary \nName for Customer Id 99: {name}");
        }
        else
        {
            Console.WriteLine("\n3. Dictionary \nKey not found");
        }

        // 4. HashSet
        // Excludes order 105 as the customer id not exists in HashSet
        HashSet<int> hash = [];
        foreach (var c in customers)
        {
            hash.Add(c.Id);
        }
        // orders.Select will transform each item into bool TRUE/FALSE. Hence, orders.Where should be used instead.
        var filteredOrders = orders.Where(n => hash.Contains(n.CustomerId));
        Console.WriteLine("\nQ4. HashSet");
        foreach (var row in filteredOrders)
        {
            Console.WriteLine($"{row.Id}. {row.CustomerId} - {row.ProductName} - {row.Amount:C2}");
        }
    }
}

class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public required string ProductName { get; set; }
    public decimal Amount { get; set; }
}