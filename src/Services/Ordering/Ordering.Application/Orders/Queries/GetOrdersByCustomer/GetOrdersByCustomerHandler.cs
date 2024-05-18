using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.Extensions;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Queries.GetOrdersByCustomer;

public class GetOrdersByCustomerHandler : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersByCustomerHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
    {
        var result = await _context.Orders
            .Include(x => x.OrderItems)
            .AsNoTracking()
            .Where(x => x.CustomerId == CustomerId.Of(query.CustomerId))
            .OrderBy(x => x.OrderName.Value)
            .ToListAsync(cancellationToken: cancellationToken);

        return new GetOrdersByCustomerResult(result.ProjectToOrdersDto());
    }
}