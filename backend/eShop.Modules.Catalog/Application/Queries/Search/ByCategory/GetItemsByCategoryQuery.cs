using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Abstractions.Primitives;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries.Search.ByCategory;

public class GetItemsByCategoryQuery : IRequest<Result<List<ProductDto>>>
{
    public string Category { get; }

    public GetItemsByCategoryQuery(string category)
    {
        Category = category;
    }
}