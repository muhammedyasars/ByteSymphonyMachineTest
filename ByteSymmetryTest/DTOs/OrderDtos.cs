using System.ComponentModel.DataAnnotations;

namespace ByteSymmetryTest.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
}

public class CreateOrderRequest
{
    [Required(ErrorMessage = "product ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "invalid product ID")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "quantity must be at least 1")]
    public int Qty { get; set; }
}
