using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ByteSymmetryTest.DTOs;
using ByteSymmetryTest.Services;

namespace ByteSymmetryTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

 
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        Guid? userId = null;

       
        if (userRole != "Admin")
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }
        }

        var orders = await _orderService.GetOrdersAsync(userId);
        return Ok(orders);
    }

 
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        Guid? userId = null;
 
        if (userRole != "Admin")
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }
        }

        var (success, data, message) = await _orderService.GetOrderByIdAsync(id, userId);

        if (!success)
        {
            return NotFound(new { errors = new[] { message } });
        }

        return Ok(data);
    }

 
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { errors });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(new { errors = new[] { "Invalid user ID" } });
        }

        var (success, data, message) = await _orderService.CreateOrderAsync(request, userId);

        if (!success)
        {
            return BadRequest(new { errors = new[] { message } });
        }

        return CreatedAtAction(nameof(GetOrder), new { id = data!.Id }, data);
    }
 
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(new { errors = new[] { "Invalid user ID" } });
        }

        var isAdmin = userRole == "Admin";
        var (success, message) = await _orderService.DeleteOrderAsync(id, userId, isAdmin);

        if (!success)
        {
            if (message.Contains("not found"))
                return NotFound(new { errors = new[] { message } });

            return BadRequest(new { errors = new[] { message } });
        }

        return Ok(new { message });
    }
}
