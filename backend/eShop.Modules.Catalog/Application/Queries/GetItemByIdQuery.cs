using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Abstractions.Primitives;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries;

public record GetItemByIdQuery(int ItemId) : IRequest<Result<ProductDto>>;