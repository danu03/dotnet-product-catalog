using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Web.Data;
using ProductCatalog.Web.DTOs;
using ProductCatalog.Web.Models;

namespace ProductCatalog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all products.");
        var products = await _context.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving product with ID {Id}.", id);
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {Id} was not found.", id);
            return NotFound(new { message = $"Product with ID {id} was not found." });
        }

        return Ok(product);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<Product>>> Search(
        [FromQuery] ProductSearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching products with criteria: Name={Name}, MinPrice={MinPrice}, MaxPrice={MaxPrice}",
            request.Name, request.MinPrice, request.MaxPrice);

        var query = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var searchName = request.Name.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchName));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        var results = await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
        return Ok(results);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product with name {Name}.", request.Name);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created successfully with ID {Id}.", product.Id);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> Update(
        int id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product with ID {Id}.", id);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {Id} not found for update.", id);
            return NotFound(new { message = $"Product with ID {id} was not found." });
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product with ID {Id} updated successfully.", id);
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product with ID {Id}.", id);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {Id} not found for deletion.", id);
            return NotFound(new { message = $"Product with ID {id} was not found." });
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product with ID {Id} deleted successfully.", id);
        return NoContent();
    }
}
