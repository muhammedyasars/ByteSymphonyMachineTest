using System.ComponentModel.DataAnnotations;

namespace ByteSymmetryTest.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductRequest
{
    [Required(ErrorMessage = "product name is required")]
    [MaxLength(200, ErrorMessage = "name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "stock cannot be negative")]
    public int Stock { get; set; }
}

public class UpdateProductRequest
{
    [Required(ErrorMessage = "product name is required")]
    [MaxLength(200, ErrorMessage = "name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "stock cannot be negative")]
    public int Stock { get; set; }
}

public class ProductListResponse
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
