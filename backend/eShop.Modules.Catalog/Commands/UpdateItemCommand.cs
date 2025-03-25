using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace eShop.Modules.Catalog.Commands;

public record UpdateItemCommand(int ItemId, UpdateProductDto Model, IFormFile? Image) : IRequest<Result<string>>;