using ByteSymmetryTest.DTOs;

namespace ByteSymmetryTest.Services;

public interface IAuthService
{
    Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
    Task<(bool Success, LoginResponse? Data, string Message)> LoginAsync(LoginRequest request);
}
