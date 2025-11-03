using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ByteSymmetryTest.Data;
using ByteSymmetryTest.DTOs;
using ByteSymmetryTest.Models;

namespace ByteSymmetryTest.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
    {
         
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return (false, "Email already exists");
        }

      
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, "User registered successfully");
    }

    public async Task<(bool Success, LoginResponse? Data, string Message)> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return (false, null, "Invalid email or password");
        }

        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return (false, null, "Invalid email or password");
        }

        
        var token = GenerateJwtToken(user);
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var response = new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        return (true, response, "Login successful");
    }

   
    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
