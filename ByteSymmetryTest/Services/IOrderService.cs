using ByteSymmetryTest.DTOs;

namespace ByteSymmetryTest.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrdersAsync(Guid? userId = null);
    Task<(bool Success, OrderDto? Data, string Message)> GetOrderByIdAsync(int id, Guid? userId = null);
    Task<(bool Success, OrderDto? Data, string Message)> CreateOrderAsync(CreateOrderRequest request, Guid userId);
    Task<(bool Success, string Message)> DeleteOrderAsync(int id, Guid userId, bool isAdmin);
}
