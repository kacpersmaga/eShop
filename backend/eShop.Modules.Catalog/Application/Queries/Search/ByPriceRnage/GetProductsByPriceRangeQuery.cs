using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Abstractions.Primitives;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries.Search.ByPriceRnage;

public class GetProductsByPriceRangeQuery : IRequest<Result<List<ProductDto>>>
{
    public decimal MinPrice { get; }
    public decimal MaxPrice { get; }

    public GetProductsByPriceRangeQuery(decimal minPrice, decimal maxPrice)
    {
        MinPrice = minPrice;
        MaxPrice = maxPrice;
    }
}