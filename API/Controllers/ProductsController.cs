using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public string GetProducts()
        {
            return "All products";
        }

        [HttpGet("{id}")]
        public string GetProduct(int id)
        {
            return $"Product {id}";
        }
    }
}