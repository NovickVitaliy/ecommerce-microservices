namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(ILogger<DeleteProductCommandHandler> logger, IDocumentSession documentSession)
    {
        _logger = logger;
        _documentSession = documentSession;
    }

    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteProductCommandHandler.Handle called with {@Command}", command);
        
        _documentSession.Delete<Product>(command.Id);

        await _documentSession.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}