using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Domain.Events.Handlers;

public class ProductUpdatedDomainEventHandler : INotificationHandler<ProductUpdatedDomainEvent>
{
    private readonly ILogger<ProductUpdatedDomainEventHandler> _logger;
    
    public ProductUpdatedDomainEventHandler(ILogger<ProductUpdatedDomainEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(ProductUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Product updated: {ProductId}, Name: {ProductName}", 
            notification.Product.Id, 
            notification.Product.Name);
        
        return Task.CompletedTask;
    }
}