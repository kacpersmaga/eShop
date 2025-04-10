using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Domain.Events.Handlers;

public class ProductPriceChangedDomainEventHandler : INotificationHandler<ProductPriceChangedDomainEvent>
{
    private readonly ILogger<ProductPriceChangedDomainEventHandler> _logger;
    
    public ProductPriceChangedDomainEventHandler(ILogger<ProductPriceChangedDomainEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(ProductPriceChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Product price changed: {ProductId}, Name: {ProductName}, Old Price: {OldValue} {OldCurrency}, New Price: {NewValue} {NewCurrency}", 
            notification.Product.Id, 
            notification.Product.Name,
            notification.OldPrice.Value,
            notification.OldPrice.Currency,
            notification.NewPrice.Value,
            notification.NewPrice.Currency);
        
        return Task.CompletedTask;
    }
}