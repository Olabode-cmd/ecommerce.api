using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;
using EcommerceApi.Models;
using EcommerceApi.DTOs;
using EcommerceApi.Services;
using System.Text.Json;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public ProductsController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Description,
                p.Rating,
                p.CategoryId,
                CategoryName = p.Category.Name,
                Tags = JsonSerializer.Deserialize<List<string>>(p.Tags ?? "[]"),
                p.ImageUrl,
                p.CreatedAt
            });

            return Ok(result);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string imageUrl = null;
            if (dto.Image != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image);
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Tags = JsonSerializer.Serialize(dto.Tags),
                ImageUrl = imageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.Rating,
                product.CategoryId,
                CategoryName = product.Category.Name,
                Tags = JsonSerializer.Deserialize<List<string>>(product.Tags ?? "[]"),
                product.ImageUrl,
                product.CreatedAt
            });
        }
    }
}
