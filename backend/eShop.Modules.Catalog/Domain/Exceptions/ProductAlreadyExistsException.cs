namespace eShop.Modules.Catalog.Domain.Exceptions;

public class ProductAlreadyExistsException : ProductDomainException
{
    public ProductAlreadyExistsException(string name)
        : base($"Product with name '{name}' already exists.") { }
}