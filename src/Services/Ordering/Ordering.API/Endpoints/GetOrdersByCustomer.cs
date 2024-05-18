using Carter;
using Mapster;
using MediatR;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints;

public record GetOrdersByCustomerRequest(Guid CustomerId);

public record GetOrdersByCustomerResponse(IEnumerable<OrderDto> Orders);

public class GetOrdersByCustomer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/customer/{customerId:guid}", async ([AsParameters]GetOrdersByCustomerRequest request, ISender sender) =>
        {
            var cmd = request.Adapt<GetOrdersByCustomerQuery>();

            var result = await sender.Send(cmd);

            var response = result.Adapt<GetOrdersResponse>();

            return Results.Ok(response);
        })
        .WithName("GetOrdersByCustomer")
        .Produces<GetOrdersByCustomerResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Orders By Customer")
        .WithDescription("Get Orders By Customer");
    }
}