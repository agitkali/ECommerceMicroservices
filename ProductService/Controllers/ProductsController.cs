using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ProductsController(ProductDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products =
                await _context.Products
                    .AsNoTracking()
                    .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProduct(
            Guid id)
        {
            var product =
                await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
        Product product)
        {
            product.Id = Guid.NewGuid();

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return Ok(product);
        }

    }
}
