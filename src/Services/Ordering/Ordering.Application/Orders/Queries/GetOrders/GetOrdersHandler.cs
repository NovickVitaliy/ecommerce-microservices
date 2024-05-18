using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.Dtos;
using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries.GetOrders;

public class GetOrdersHandler : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrdersResult> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PaginationRequest.PageIndex;
        var pageSize = request.PaginationRequest.PageSize;

        var totalCount = await _context.Orders.LongCountAsync(cancellationToken: cancellationToken);

        var orders = await _context.Orders
            .Include(x => x.OrderItems)
            .OrderBy(x => x.OrderName.Value)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new GetOrdersResult(new PaginatedResult<OrderDto>(
            pageIndex, 
            pageSize, 
            totalCount, 
            orders.ProjectToOrdersDto()));
    }
}