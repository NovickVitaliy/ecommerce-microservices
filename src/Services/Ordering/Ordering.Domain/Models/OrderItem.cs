namespace Ordering.Domain.Models;

public class OrderItem : Entity<OrderItemId>
{
    internal OrderItem(OrderId orderId, ProductId productId, int quantity, decimal price)
    {
        Id = OrderItemId.Of(Guid.NewGuid());
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    public OrderId OrderId { get; } = default;
    public ProductId ProductId { get; } = default;
    public int Quantity { get; } = default;
    public decimal Price { get; } = default;
}