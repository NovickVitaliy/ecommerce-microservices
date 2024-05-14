using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly DiscountContext _context;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(DiscountContext context, ILogger<DiscountService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

        if (coupon is null)
        {
            coupon = new Coupon() { ProductName = "No Discount", Amount = 0, Description = "No Discount" };
        }

        _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}",
            coupon.ProductName, coupon.Amount);
        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();

        if (coupon is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
        }

        await _context.Coupons.AddAsync(coupon);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();

        if (coupon is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
        }

        _context.Coupons.Update(coupon);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,
        ServerCallContext context)
    {
        var coupon = await _context
            .Coupons
            .FirstOrDefaultAsync(x => x.ProductName == request.Coupon.ProductName);

        if (coupon is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"Discount with ProductName={request.Coupon.ProductName} is not found."));
        }

        _context.Coupons.Remove(coupon);
        var rowsAffected = await _context.SaveChangesAsync();
        
        _logger.LogInformation("Discount is successfully deleted. ProductName: {ProductName}", request.Coupon.ProductName);

        return new DeleteDiscountResponse()
        {
            Success = rowsAffected > 0
        };
    }
}