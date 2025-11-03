using Microsoft.EntityFrameworkCore;
using ByteSymmetryTest.Data;
using ByteSymmetryTest.DTOs;
using ByteSymmetryTest.Models;

namespace ByteSymmetryTest.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> GetOrdersAsync(Guid? userId = null)
    {
        var query = _context.Orders.Include(o => o.Product).AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        var orders = await query
            .Select(o => new OrderDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                ProductName = o.Product.Name,
                Qty = o.Qty,
                Total = o.Total,
                OrderDate = o.OrderDate
            })
            .ToListAsync();

        return orders;
    }

    public async Task<(bool Success, OrderDto? Data, string Message)> GetOrderByIdAsync(int id, Guid? userId = null)
    {
        var query = _context.Orders.Include(o => o.Product).AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        var order = await query.FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return (false, null, "Order not found");
        }

        var dto = new OrderDto
        {
            Id = order.Id,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            Qty = order.Qty,
            Total = order.Total,
            OrderDate = order.OrderDate
        };

        return (true, dto, "Order retrieved successfully");
    }

 
    public async Task<(bool Success, OrderDto? Data, string Message)> CreateOrderAsync(CreateOrderRequest request, Guid userId)
    {
        
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product == null)
            {
                return (false, null, "Product not found");
            }

            
            if (product.Stock < request.Qty)
            {
                return (false, null, $"Insufficient stock. Available: {product.Stock}, Requested: {request.Qty}");
            }

           
            product.Stock -= request.Qty;

           
            var total = product.Price * request.Qty;

           
            var order = new Order
            {
                ProductId = request.ProductId,
                UserId = userId,
                Qty = request.Qty,
                Total = total,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);

          
            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return (false, null, "Product was modified by another transaction. Please try again.");
            }

            var dto = new OrderDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                ProductName = product.Name,
                Qty = order.Qty,
                Total = order.Total,
                OrderDate = order.OrderDate
            };

            return (true, dto, "Order created successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, null, $"Error creating order: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> DeleteOrderAsync(int id, Guid userId, bool isAdmin)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return (false, "Order not found");
        }

       
        if (!isAdmin && order.UserId != userId)
        {
            return (false, "Unauthorized to delete this order");
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return (true, "Order deleted successfully");
    }
}
