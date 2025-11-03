using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteSymmetryTest.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "quantity must be greater than 0")]
    public int Qty { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Total { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
