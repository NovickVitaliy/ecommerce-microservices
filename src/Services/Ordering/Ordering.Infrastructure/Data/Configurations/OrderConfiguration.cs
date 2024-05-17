using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Enums;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                orderId => orderId.Value,
                dbId => OrderId.Of(dbId));

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .IsRequired();

        builder.HasMany(x => x.OrderItems)
            .WithOne()
            .HasForeignKey(x => x.OrderId);

        builder.ComplexProperty(x => x.OrderName, propertyBuilder =>
        {
            propertyBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Order.OrderName))
                .HasMaxLength(100)
                .IsRequired();
        });
        
        builder.ComplexProperty(x => x.ShippingAddress, propertyBuilder =>
        {
            propertyBuilder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            propertyBuilder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            propertyBuilder.Property(x => x.EmailAddress)
                .HasMaxLength(180)
                .IsRequired();

            propertyBuilder.Property(x => x.Country)
                .HasMaxLength(50);

            propertyBuilder.Property(x => x.State)
                .HasMaxLength(50);

            propertyBuilder.Property(x => x.ZipCode)
                .HasMaxLength(50)
                .IsRequired();
        });
        
        builder.ComplexProperty(x => x.BillingAddress, propertyBuilder =>
        {
            propertyBuilder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            propertyBuilder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            propertyBuilder.Property(x => x.EmailAddress)
                .HasMaxLength(180)
                .IsRequired();

            propertyBuilder.Property(x => x.Country)
                .HasMaxLength(50);

            propertyBuilder.Property(x => x.State)
                .HasMaxLength(50);

            propertyBuilder.Property(x => x.ZipCode)
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.ComplexProperty(x => x.Payment, propertyBuilder =>
        {
            propertyBuilder.Property(x => x.CardName)
                .HasMaxLength(50);

            propertyBuilder.Property(x => x.CardNumber)
                .HasMaxLength(50)
                .IsRequired();

            propertyBuilder.Property(x => x.Expiration)
                .HasMaxLength(10);

            propertyBuilder.Property(x => x.CVV)
                .HasMaxLength(3);

            propertyBuilder.Property(x => x.PaymentMethod);
        });

        builder.Property(x => x.Status)
            .HasDefaultValue(OrderStatus.Draft)
            .HasConversion(x => x.ToString(),
                dbStatus => Enum.Parse<OrderStatus>(dbStatus));

        builder.Property(x => x.TotalPrice);
    }
}