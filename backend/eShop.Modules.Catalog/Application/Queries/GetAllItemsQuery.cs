using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Common;
using MediatR;

namespace eShop.Modules.Catalog.Application.Queries;

public record GetAllItemsQuery : IRequest<Result<List<ProductDto>>>;