namespace Catalog.API.Products.GetProductsByCategory;

public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;

public record GetProductsByCategoryResult(IEnumerable<Product> Products);

public class GetProductsByCategoryQueryHandler : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<GetProductsByCategoryQueryHandler> _logger;

    public GetProductsByCategoryQueryHandler(IDocumentSession documentSession, ILogger<GetProductsByCategoryQueryHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetProductsByCategoryQueryHandler.Handle called with {@Query}", query);

        var result = await _documentSession.Query<Product>()
            .Where(x => x.Category.Contains(query.Category))
            .ToListAsync(token: cancellationToken);

        return new GetProductsByCategoryResult(result);
    }
}