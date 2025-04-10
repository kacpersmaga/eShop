namespace eShop.Modules.Catalog.Domain.Exceptions;

public class ProductNotFoundException : ProductDomainException
{
    public ProductNotFoundException(int id) 
        : base($"Product with ID {id} was not found.") { }
}