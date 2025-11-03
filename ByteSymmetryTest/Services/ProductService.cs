using Microsoft.EntityFrameworkCore;
using ByteSymmetryTest.Data;
using ByteSymmetryTest.DTOs;
using ByteSymmetryTest.Models;

namespace ByteSymmetryTest.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductListResponse> GetProductsAsync(int page, int pageSize, string? search, string? sort)
    {
        var query = _context.Products.AsQueryable();

         
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

       
        var totalCount = await query.CountAsync();
 
        query = sort?.ToLower() switch
        {
            "name" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            "price" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Id)  
        };

       
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new ProductListResponse
        {
            Products = products,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<(bool Success, ProductDto? Data, string Message)> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return (false, null, "Product not found");
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };

        return (true, dto, "Product retrieved successfully");
    }

    public async Task<(bool Success, ProductDto? Data, string Message)> CreateProductAsync(CreateProductRequest request)
    {
         
        if (await _context.Products.AnyAsync(p => p.Name == request.Name))
        {
            return (false, null, "Product name already exists");
        }

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };

        return (true, dto, "Product created successfully");
    }

    public async Task<(bool Success, ProductDto? Data, string Message)> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return (false, null, "Product not found");
        }

        
        if (await _context.Products.AnyAsync(p => p.Name == request.Name && p.Id != id))
        {
            return (false, null, "Product name already exists");
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return (false, null, "Product was modified by another user. Please refresh and try again.");
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };

        return (true, dto, "Product updated successfully");
    }

    public async Task<(bool Success, string Message)> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return (false, "Product not found");
        }

         
        var hasOrders = await _context.Orders.AnyAsync(o => o.ProductId == id);
        if (hasOrders)
        {
            return (false, "Cannot delete product with existing orders");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return (true, "Product deleted successfully");
    }
}
