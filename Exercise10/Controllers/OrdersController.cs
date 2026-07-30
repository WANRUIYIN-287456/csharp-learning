using System.Data;
using Exercise10.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public class OrdersController : Controller
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public OrdersController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders.Include(o => o.Customer)
            .Include(o => o.Product).ToListAsync();

        return View(orders);
    }

    [HttpGet("Orders/RawByCustomer/{customerId}")]
    public async Task<IActionResult> RawByCustomer(int customerId)
    {
        string connStr = _config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var results = new List<dynamic>();

        using var connection = new SqlConnection(connStr);
        await connection.OpenAsync();

        using var command = new SqlCommand("GetCustomerOrders", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@CustomerId", customerId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new
            {
                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                OrderValue = reader.GetDecimal(reader.GetOrdinal("OrderValue"))
            });
        }
        return Json(results);
    }
}