namespace eShop.Modules.Catalog.Domain.Exceptions;

public class InvalidProductDataException : ProductDomainException 
{
    public InvalidProductDataException(string message) 
        : base(message) { }
}