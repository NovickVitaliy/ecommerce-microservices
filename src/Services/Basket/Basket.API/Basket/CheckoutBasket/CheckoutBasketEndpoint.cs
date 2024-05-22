using Basket.API.Dto;
using OpenTelemetry.Trace;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);

public record CheckoutBasketResponse(bool IsSuccess);

public class CheckoutBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender) =>
        {
            var cmd = request.Adapt<CheckoutBasketCommand>();

            var result = await sender.Send(cmd);

            var response = result.Adapt<CheckoutBasketResponse>();

            return Results.Ok(response);
        })
        .WithName("CheckoutBasket")
        .Produces<CheckoutBasketResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Checkout Basket")
        .WithDescription("Checkout Basket");
    }
}