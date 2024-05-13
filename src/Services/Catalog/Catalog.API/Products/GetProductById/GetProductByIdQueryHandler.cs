namespace Catalog.API.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

public record GetProductByIdResult(Product Product);

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    private readonly IDocumentSession _documentSession;
    public GetProductByIdQueryHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _documentSession.LoadAsync<Product>(query.Id, cancellationToken);
        
        if (result is null) throw new ProductNotFoundException(query.Id);

        return new GetProductByIdResult(result);
    }
}