using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.Exceptions;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.DeleteOrder;

public class DeleteOrderHandler : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync([OrderId.Of(command.Id)], cancellationToken);
        if (order is null)
        {
            throw new OrderNotFoundException(command.Id);
        }

        _context.Orders.Remove(order);
        var result = await _context.SaveChangesAsync(cancellationToken);

        return new DeleteOrderResult(result > 0);
    }
}