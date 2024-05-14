using Discount.Grpc;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart)
            .NotNull().WithMessage("Cart can not be null");

        RuleFor(x => x.Cart.UserName)
            .NotEmpty().WithMessage("UserName is required");
    }
}

public class StoreBaskerCommandHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    private readonly IBasketRepository _basketRepository;
    private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;
    public StoreBaskerCommandHandler(IBasketRepository basketRepository, DiscountProtoService.DiscountProtoServiceClient discountProtoService)
    {
        _basketRepository = basketRepository;
        _discountProtoService = discountProtoService;
    }

    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        await DeductDiscount(command.Cart, cancellationToken);
        
        await _basketRepository.StoreBasket(command.Cart, cancellationToken);

        return new StoreBasketResult(command.Cart.UserName);
    }

    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            var coupon = await _discountProtoService.GetDiscountAsync(new GetDiscountRequest()
                { ProductName = item.ProductName }, cancellationToken: cancellationToken);
            
            item.Price -= coupon.Amount;
        }
    }
}