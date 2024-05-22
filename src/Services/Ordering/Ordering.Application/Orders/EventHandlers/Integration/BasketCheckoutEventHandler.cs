using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.EventHandlers.Integration;

public class BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<BasketCheckoutEventHandler> _logger;

    public BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        _logger.LogInformation("Integration event handler: {IntegrationEvent}", context.Message.GetType().Name);

        var command = MapToCreateOrderCommand(context.Message);
        await _sender.Send(command);
    }

    private static CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent msg)
    {
        var addressDto = new AddressDto(msg.FirstName, msg.LastName, msg.EmailAdress, msg.AddressLine, msg.Country, msg.State, msg.ZipCode);
        var paymentDto = new PaymentDto(msg.CardName, msg.CardNumber, msg.Expiration, msg.CVV, msg.PaymentMethod);
        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto(
                orderId,
                msg.CustomerId,
                msg.UserName,
                addressDto,
                addressDto,
                paymentDto,
                OrderStatus.Pending,
                [
                    new OrderItemDto(orderId, Guid.NewGuid(), 2, 500),
                    new OrderItemDto(orderId, Guid.NewGuid(), 1, 400)
                ]);

        return new CreateOrderCommand(orderDto);
    }
}