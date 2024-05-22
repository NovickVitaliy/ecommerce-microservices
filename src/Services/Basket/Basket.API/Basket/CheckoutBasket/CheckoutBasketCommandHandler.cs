using Basket.API.Dto;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto) : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto)
            .NotNull().WithMessage("BacketCheckoutDto can't be null");
        RuleFor(x => x.BasketCheckoutDto.UserName)
            .NotEmpty().WithMessage("UserName is required");
    }
}

public class CheckoutBasketCommandHandler : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CheckoutBasketCommandHandler(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint)
    {
        _basketRepository = basketRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
        if (basket is null)
        {
            return new CheckoutBasketResult(false);
        }

        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.TotalPrice = basket.TotalPrice;

        await _publishEndpoint.Publish(eventMessage, cancellationToken);

        await _basketRepository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken);

        return new CheckoutBasketResult(true);
    }
}