using System.ComponentModel.Design;
using Exercise9.Data;
using Exercise9.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Add a list item in the layout navitems so that Product appears together with 
public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;    
    }

    // GET /Products
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        var viewModel = new ProductListViewModel
        {
            Products = products,
            TotalStockValue = products.Sum( p => p.Price * p.Stock ),
        };
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create() => View();
   
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    
}