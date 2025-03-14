namespace eShop.Modules.Catalog.Application.Dtos;

public class CreateProductDto
{
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public required string Category { get; init; }
}