namespace eShop.Modules.Catalog.Application.Dtos;

public class PagedProductsDto
{
    public List<ProductDto> Items { get; set; } = new List<ProductDto>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}