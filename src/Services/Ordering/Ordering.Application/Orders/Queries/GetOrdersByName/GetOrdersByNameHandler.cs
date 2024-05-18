using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.Dtos;
using Ordering.Application.Extensions;
using Ordering.Domain.Models;

namespace Ordering.Application.Orders.Queries.GetOrdersByName;

public class GetOrdersByNameHandler : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersByNameHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Include(x => x.OrderItems)
            .AsNoTracking()
            .Where(x => x.OrderName.Value.Contains(query.Name))
            .OrderBy(x => x.OrderName)
            .ToListAsync(cancellationToken);

        var orderDtos = orders.ProjectToOrdersDto();

        return new GetOrdersByNameResult(orderDtos);
    }
}