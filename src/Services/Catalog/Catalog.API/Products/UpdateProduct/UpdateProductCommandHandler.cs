namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    IEnumerable<string> Category,
    string Description,
    string ImageFile,
    decimal Price) : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IDocumentSession documentSession, ILogger<UpdateProductCommandHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateProductCommandHandler.Handle called with {@Command}", command);

        var product = await _documentSession.LoadAsync<Product>(command.Id, cancellationToken);

        if (product is null) throw new ProductNotFoundException();

        product.Name = command.Name;
        product.Description = command.Description;
        product.Category = command.Category.ToList();
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;
        
        _documentSession.Update(product);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}