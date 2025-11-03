using System.ComponentModel.DataAnnotations;

namespace ByteSymmetryTest.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "full name is required")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "email is required")]
    [EmailAddress(ErrorMessage = "invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "password is required")]
    [MinLength(8, ErrorMessage = "password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "role is required")]
    [RegularExpression("^(Admin|User)$", ErrorMessage = "role must be either Admin or User")]
    public string Role { get; set; } = "User";
}

public class LoginRequest
{
    [Required(ErrorMessage = "email is required")]
    [EmailAddress(ErrorMessage = "invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "password is required")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
