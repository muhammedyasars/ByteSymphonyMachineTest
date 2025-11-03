using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ByteSymmetryTest.DTOs;
using ByteSymmetryTest.Services;

namespace ByteSymmetryTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

 
    [HttpGet]
    [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _productService.GetProductsAsync(page, pageSize, search, sort);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int id)
    {
        var (success, data, message) = await _productService.GetProductByIdAsync(id);

        if (!success)
        {
            return NotFound(new { errors = new[] { message } });
        }

        return Ok(data);
    }

 
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { errors });
        }

        var (success, data, message) = await _productService.CreateProductAsync(request);

        if (!success)
        {
            return BadRequest(new { errors = new[] { message } });
        }

        return CreatedAtAction(nameof(GetProduct), new { id = data!.Id }, data);
    }

 
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { errors });
        }

        var (success, data, message) = await _productService.UpdateProductAsync(id, request);

        if (!success)
        {
            if (message.Contains("not found"))
                return NotFound(new { errors = new[] { message } });

            return BadRequest(new { errors = new[] { message } });
        }

        return Ok(data);
    }

 
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var (success, message) = await _productService.DeleteProductAsync(id);

        if (!success)
        {
            if (message.Contains("not found"))
                return NotFound(new { errors = new[] { message } });

            return BadRequest(new { errors = new[] { message } });
        }

        return Ok(new { message });
    }
}
