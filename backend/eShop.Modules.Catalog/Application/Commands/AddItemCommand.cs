﻿using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace eShop.Modules.Catalog.Application.Commands;

public record AddItemCommand(CreateProductDto Model, IFormFile? Image) : IRequest<Result<string>>;