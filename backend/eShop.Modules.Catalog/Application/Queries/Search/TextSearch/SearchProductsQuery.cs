using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Abstractions.Primitives;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries.Search.TextSearch;

public class SearchProductsQuery : IRequest<Result<List<ProductDto>>>
{
    public string SearchTerm { get; }

    public SearchProductsQuery(string searchTerm)
    {
        SearchTerm = searchTerm;
    }
}