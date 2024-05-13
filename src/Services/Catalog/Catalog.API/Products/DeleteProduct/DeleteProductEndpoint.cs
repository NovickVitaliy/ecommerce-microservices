using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductRequest(Guid Id);

public record DeleteProductResponse(bool IsSuccess);

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/products/{id:guid}", async (Guid id, ISender sender) =>
        {
            
            var result = await sender.Send(new DeleteProductCommand(id));
            
            var response = result.Adapt<DeleteProductResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteProduct")
        .Produces<DeleteProductResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Delete Product")
        .WithDescription("Delete Product");
    }
}