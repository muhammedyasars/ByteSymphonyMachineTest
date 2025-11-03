using ByteSymmetryTest.DTOs;
using ByteSymmetryTest.Models;

namespace ByteSymmetryTest.Services;

public interface IProductService
{
    Task<ProductListResponse> GetProductsAsync(int page, int pageSize, string? search, string? sort);
    Task<(bool Success, ProductDto? Data, string Message)> GetProductByIdAsync(int id);
    Task<(bool Success, ProductDto? Data, string Message)> CreateProductAsync(CreateProductRequest request);
    Task<(bool Success, ProductDto? Data, string Message)> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<(bool Success, string Message)> DeleteProductAsync(int id);
}
