using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.DTOS;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ProductDbContext _context;

    public ProductController(ProductDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var productList = new List<ProductDetailDTO>();
        var products = await _context.Products.ToListAsync();
        foreach (var product in products)
        {
            var productDetails = new ProductDetailDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.InventoryStock
            };
            productList.Add(productDetails);
        }
        return Ok(productList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        var productDetails = new ProductDetailDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.InventoryStock
        };
        return Ok(productDetails);
    }


    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDTO productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            InventoryStock = productDto.Stock
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        var productResponse = new ProductCreateResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.InventoryStock
        };
        return Ok(productResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductCreateDTO updatedProductDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        product.Name = updatedProductDto.Name;
        product.Price = updatedProductDto.Price;
        product.InventoryStock = updatedProductDto.Stock;
        await _context.SaveChangesAsync();
        var productResponse = new ProductCreateResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.InventoryStock
        };
        return Ok(productResponse);
    }
}