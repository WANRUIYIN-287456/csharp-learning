using System.ComponentModel.Design;
using Exercise13.Data;
using Exercise13.Models;
using Exercise13.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Add a list item in the layout navitems so that Product appears together with 
public class ProductsController : Controller
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;    
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = await _service.GetProductListAsync();
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create() => View();
   
    [Authorize(Roles ="Manager")]
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid) return View(product);

        await _service.CreateProductAsync(product);
        return RedirectToAction("Index");
    }
    
}