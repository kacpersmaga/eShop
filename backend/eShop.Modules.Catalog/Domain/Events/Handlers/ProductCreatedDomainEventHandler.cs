using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Domain.Events.Handlers;

public class ProductCreatedDomainEventHandler : INotificationHandler<ProductCreatedDomainEvent>
{
    private readonly ILogger<ProductCreatedDomainEventHandler> _logger;
    
    public ProductCreatedDomainEventHandler(ILogger<ProductCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Product created: {ProductId}, Name: {ProductName}", 
            notification.Product.Id, 
            notification.Product.Name);
        
        return Task.CompletedTask;
    }
}