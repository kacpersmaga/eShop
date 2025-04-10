using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Domain.Events.Handlers;

public class ProductStatusChangedDomainEventHandler : INotificationHandler<ProductStatusChangedDomainEvent>
{
    private readonly ILogger<ProductStatusChangedDomainEventHandler> _logger;
    
    public ProductStatusChangedDomainEventHandler(ILogger<ProductStatusChangedDomainEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(ProductStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Product availability status changed: {ProductId}, Name: {ProductName}, Is Available: {IsAvailable}", 
            notification.Product.Id, 
            notification.Product.Name,
            notification.IsAvailable);
        
        return Task.CompletedTask;
    }
}