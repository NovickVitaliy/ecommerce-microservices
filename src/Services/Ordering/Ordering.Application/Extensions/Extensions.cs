using Ordering.Application.Dtos;
using Ordering.Domain.Models;

namespace Ordering.Application.Extensions;

public static class Extensions
{
    public static IEnumerable<OrderDto> ProjectToOrdersDto(this IEnumerable<Order> orders)
        => orders.Select(x => new OrderDto(
            x.Id.Value,
            x.CustomerId.Value,
            x.OrderName.Value,
            new AddressDto(
                x.ShippingAddress.FirstName,
                x.ShippingAddress.LastName,
                x.ShippingAddress.EmailAddress,
                x.ShippingAddress.AddressLine,
                x.ShippingAddress.Country,
                x.ShippingAddress.State,
                x.ShippingAddress.ZipCode
            ),
            new AddressDto(
                x.BillingAddress.FirstName,
                x.BillingAddress.LastName,
                x.BillingAddress.EmailAddress,
                x.BillingAddress.AddressLine,
                x.BillingAddress.Country,
                x.BillingAddress.State,
                x.BillingAddress.ZipCode
            ),
            new PaymentDto(
                x.Payment.CardName,
                x.Payment.CardNumber,
                x.Payment.Expiration,
                x.Payment.CVV,
                x.Payment.PaymentMethod
            ),
            x.Status,
            x.OrderItems.Select(oi => new OrderItemDto(
                oi.OrderId.Value, 
                oi.ProductId.Value,
                oi.Quantity,
                oi.Price)).ToList()
        ));
}