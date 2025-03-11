using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Common;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries;

public record GetItemByIdQuery(int ItemId) : IRequest<Result<ShopItemViewModel>>;