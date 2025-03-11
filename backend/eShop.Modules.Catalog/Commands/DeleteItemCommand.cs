﻿using eShop.Shared.Common;
using MediatR;

namespace eShop.Modules.Catalog.Commands;

public record DeleteItemCommand(int ItemId) : IRequest<Result<string>>;