using Exercise7.Models;
using Microsoft.AspNetCore.Mvc;
// Add a list item in the layout navitems so that Product appears together with 
public class ProductsController : Controller
{
    // GET /Products
    public IActionResult Index()
    {
        var products = ProductStore.Products;
        var viewModel = new ProductListViewModel
        {
            Products = products,
            TotalStockValue = products.Sum( p => p.Price * p.Stock ),
        };
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
   
    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var newId = ProductStore.Products.Max(p => p.Id) + 1;
        product.Id = newId;
        ProductStore.Products.Add(product);
        return RedirectToAction("Index");
    }
    
}