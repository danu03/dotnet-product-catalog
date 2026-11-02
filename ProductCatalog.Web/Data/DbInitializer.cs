using Microsoft.EntityFrameworkCore;
using ProductCatalog.Web.Models;

namespace ProductCatalog.Web.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, ILogger logger, CancellationToken cancellationToken = default)
    {
        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (!await context.Products.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Seeding initial catalog products.");
            var now = DateTime.UtcNow;
            context.Products.AddRange(
                new Product
                {
                    Name = "Office Ergonomic Desk",
                    Description = "Adjustable height standing desk with cable management tray.",
                    Price = 3750000m,
                    CreatedAt = now.AddDays(-5)
                },
                new Product
                {
                    Name = "Mechanical Keyboard",
                    Description = "Tenkeyless layout with tactile silent switches.",
                    Price = 1350000m,
                    CreatedAt = now.AddDays(-4)
                },
                new Product
                {
                    Name = "Noise-Cancelling Headphones",
                    Description = "Wireless over-ear headset with 30-hour battery life.",
                    Price = 2400000m,
                    CreatedAt = now.AddDays(-3)
                },
                new Product
                {
                    Name = "27-inch 4K Monitor",
                    Description = "IPS panel with HDR support and USB-C display input.",
                    Price = 4990000m,
                    CreatedAt = now.AddDays(-2)
                }
            );
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
