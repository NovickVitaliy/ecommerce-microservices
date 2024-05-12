namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery() : IQuery<GetProductsResult>;

public record GetProductsResult(IEnumerable<Product> Products);
    
public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<GetProductsQueryHandler> _logger;
    public GetProductsQueryHandler(IDocumentSession documentSession, ILogger<GetProductsQueryHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetProductsQueryHandler.Handler called with {@Query}", query);

        var products = await _documentSession.Query<Product>().ToListAsync(token: cancellationToken);

        return new GetProductsResult(products);
    }
}