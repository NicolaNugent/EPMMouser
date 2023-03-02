using EPM.Mouser.Interview.Data;
using EPM.Mouser.Interview.Models;
using Microsoft.AspNetCore.Mvc;

namespace EPM.Mouser.Interview.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IWarehouseRepository _warehouseRepo;

        public HomeController(IWarehouseRepository warehouseRepository)
        {
            this._warehouseRepo = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Product> products = new List<Product>();
            products = await _warehouseRepo.List();
            // Should this be only items with Available Stock?
            return View(products);
        }

        [HttpGet("Product")]
        public async Task<IActionResult> ProductPage(int id)
        {
            Product? product = await _warehouseRepo.Get(id);
            if (product == null)
            {                
                product = new Product();
                product.Name = "Error occurred";
            }
            return View(product);
        }
    }
}
