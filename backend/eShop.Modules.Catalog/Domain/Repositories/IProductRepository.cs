using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications;
using eShop.Modules.Catalog.Domain.Specifications.Builders;

namespace eShop.Modules.Catalog.Domain.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<Product?> GetByNameAsync(string name);
    Task<bool> ExistsAsync(int id);
    
    Task<Product?> GetBySpecAsync(ISpecification<Product> spec);
    Task<IEnumerable<Product>> ListAsync(ISpecification<Product> spec);
    Task<int> CountAsync(ISpecification<Product> spec);
    
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}