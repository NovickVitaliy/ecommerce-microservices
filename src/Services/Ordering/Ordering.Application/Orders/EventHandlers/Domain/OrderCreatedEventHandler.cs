using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Ordering.Application.Extensions;
using Ordering.Domain.Events;

namespace Ordering.Application.Orders.EventHandlers.Domain;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IFeatureManager _featureManager;
    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger, IPublishEndpoint publishEndpoint, IFeatureManager featureManager)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _featureManager = featureManager;
    }

    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain event handled: {DomainEvent}", domainEvent.GetType().Name);

        if (await _featureManager.IsEnabledAsync("OrderFulfillment"))
        {
            var orderCreatedIntegrationEvent = domainEvent.Order.ToOrderDto();
            await _publishEndpoint.Publish(orderCreatedIntegrationEvent, cancellationToken);
        }
    }
}