using System.ComponentModel.DataAnnotations;

namespace ByteSymmetryTest.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "User";  

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
