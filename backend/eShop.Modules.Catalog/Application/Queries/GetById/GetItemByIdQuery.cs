using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Abstractions.Primitives;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries.GetById;

public class GetItemByIdQuery : IRequest<Result<PagedProductsDto>>
{
    public int ItemId { get; }

    public GetItemByIdQuery(int itemId)
    {
        ItemId = itemId;
    }
}