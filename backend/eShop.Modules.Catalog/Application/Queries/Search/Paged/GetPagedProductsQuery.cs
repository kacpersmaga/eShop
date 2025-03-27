using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Abstractions.Primitives;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries.Search.Paged;

public class GetPagedProductsQuery : IRequest<Result<PagedProductsDto>>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public string? Category { get; }
    public string? SortBy { get; }
    public bool Ascending { get; }

    public GetPagedProductsQuery(
        int pageNumber, 
        int pageSize, 
        string? category = null, 
        string? sortBy = null, 
        bool ascending = true)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
        Category = category;
        SortBy = sortBy;
        Ascending = ascending;
    }
}