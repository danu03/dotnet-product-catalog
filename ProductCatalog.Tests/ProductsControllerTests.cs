using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ProductCatalog.Web.Controllers;
using ProductCatalog.Web.Data;
using ProductCatalog.Web.DTOs;
using ProductCatalog.Web.Models;
using Xunit;

namespace ProductCatalog.Tests;

public class ProductsControllerTests
{
    private static AppDbContext CreateInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsAllProductsOrderedByDate()
    {
        using var context = CreateInMemoryDbContext(nameof(GetAll_ReturnsAllProductsOrderedByDate));
        context.Products.AddRange(
            new Product { Name = "Product A", Description = "Desc A", Price = 10.0m, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Product { Name = "Product B", Description = "Desc B", Price = 20.0m, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var result = await controller.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Equal(2, products.Count());
        Assert.Equal("Product B", products.First().Name);
    }

    [Fact]
    public async Task GetById_WithExistingId_ReturnsOkWithProduct()
    {
        using var context = CreateInMemoryDbContext(nameof(GetById_WithExistingId_ReturnsOkWithProduct));
        var product = new Product { Name = "Product 1", Description = "Desc 1", Price = 50.0m, CreatedAt = DateTime.UtcNow };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var result = await controller.GetById(product.Id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal("Product 1", returnedProduct.Name);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        using var context = CreateInMemoryDbContext(nameof(GetById_WithNonExistentId_ReturnsNotFound));
        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var result = await controller.GetById(999, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Search_ByNameAndPriceRange_ReturnsMatchingProducts()
    {
        using var context = CreateInMemoryDbContext(nameof(Search_ByNameAndPriceRange_ReturnsMatchingProducts));
        context.Products.AddRange(
            new Product { Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 25.0m, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Gaming Mouse Pad", Description = "Extended desk mat", Price = 15.0m, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Mechanical Keyboard", Description = "RGB Keyboard", Price = 100.0m, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var request = new ProductSearchRequest
        {
            Name = "mouse",
            MinPrice = 20.0m,
            MaxPrice = 30.0m
        };

        var result = await controller.Search(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var matches = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Single(matches);
        Assert.Equal("Wireless Mouse", matches.First().Name);
    }

    [Fact]
    public async Task Create_WithValidProduct_ReturnsCreatedAtAction()
    {
        using var context = CreateInMemoryDbContext(nameof(Create_WithValidProduct_ReturnsCreatedAtAction));
        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var request = new CreateProductRequest
        {
            Name = "Standing Desk",
            Description = "Electric motorized desk",
            Price = 299.99m
        };

        var result = await controller.Create(request, CancellationToken.None);

        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdProduct = Assert.IsType<Product>(createdAtResult.Value);
        Assert.Equal("Standing Desk", createdProduct.Name);
        Assert.True(createdProduct.Id > 0);
    }

    [Fact]
    public async Task Update_WithExistingId_UpdatesProductAndReturnsOk()
    {
        using var context = CreateInMemoryDbContext(nameof(Update_WithExistingId_UpdatesProductAndReturnsOk));
        var product = new Product { Name = "Old Name", Description = "Old Desc", Price = 10.0m, CreatedAt = DateTime.UtcNow };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var updateRequest = new UpdateProductRequest
        {
            Name = "New Name",
            Description = "Updated description",
            Price = 45.0m
        };

        var result = await controller.Update(product.Id, updateRequest, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var updatedProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal("New Name", updatedProduct.Name);
        Assert.Equal(45.0m, updatedProduct.Price);
    }

    [Fact]
    public async Task Update_WithNonExistentId_ReturnsNotFound()
    {
        using var context = CreateInMemoryDbContext(nameof(Update_WithNonExistentId_ReturnsNotFound));
        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var updateRequest = new UpdateProductRequest
        {
            Name = "Any Name",
            Description = "Any Description",
            Price = 50.0m
        };

        var result = await controller.Update(999, updateRequest, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WithExistingId_RemovesProductAndReturnsNoContent()
    {
        using var context = CreateInMemoryDbContext(nameof(Delete_WithExistingId_RemovesProductAndReturnsNoContent));
        var product = new Product { Name = "Item to Delete", Description = "Desc", Price = 12.0m, CreatedAt = DateTime.UtcNow };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var result = await controller.Delete(product.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await context.Products.FindAsync(product.Id));
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ReturnsNotFound()
    {
        using var context = CreateInMemoryDbContext(nameof(Delete_WithNonExistentId_ReturnsNotFound));
        var controller = new ProductsController(context, NullLogger<ProductsController>.Instance);

        var result = await controller.Delete(999, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
