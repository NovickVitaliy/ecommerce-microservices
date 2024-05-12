namespace Catalog.API.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

public record GetProductByIdResult(Product Product);

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    
    public GetProductByIdQueryHandler(IDocumentSession documentSession, ILogger<GetProductByIdQueryHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetProductByIdHandler.Handler called with {@Query}", query);

        var result = await _documentSession.LoadAsync<Product>(query.Id, cancellationToken);
        
        if (result is null) throw new ProductNotFoundException();

        return new GetProductByIdResult(result);
    }
}