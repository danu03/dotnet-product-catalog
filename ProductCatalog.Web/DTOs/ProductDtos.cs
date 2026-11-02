using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Web.DTOs;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 120 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 999999999999.0, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }
}

public class UpdateProductRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 120 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 999999999999.0, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }
}

public class ProductSearchRequest : IValidatableObject
{
    [StringLength(100, ErrorMessage = "Search name cannot exceed 100 characters.")]
    public string? Name { get; set; }

    [Range(0.0, 999999999999.0, ErrorMessage = "Minimum price cannot be negative.")]
    public decimal? MinPrice { get; set; }

    [Range(0.0, 999999999999.0, ErrorMessage = "Maximum price cannot be negative.")]
    public decimal? MaxPrice { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice.Value > MaxPrice.Value)
        {
            yield return new ValidationResult(
                "Minimum price cannot be greater than maximum price.",
                new[] { nameof(MinPrice), nameof(MaxPrice) });
        }
    }
}
