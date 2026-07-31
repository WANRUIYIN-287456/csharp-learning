using System.Data;
using System.Diagnostics;
using Exercise11.Data;
using Exercise11.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "Name");
        ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "Name");
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
            return View(order);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product == null || order.Quantity > product.Stock)
            {
                ModelState.AddModelError("Quantity", "Insufficient stock.");
                ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "Name");
                ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
                return View(order);
            }

            product.Stock -= order.Quantity;
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("Index");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}