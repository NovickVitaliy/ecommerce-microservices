using BuildingBlocks.Behaviours;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Messaging.MassTransit;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddMarten(cfg =>
    {
        cfg.Connection(builder.Configuration.GetConnectionString("Database")
                       ?? throw new InvalidOperationException("Database connection string was not found"));
        cfg.Schema.For<ShoppingCart>().Identity(x => x.UserName);
    })
    .UseLightweightSessions();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});

builder.Services.AddCarter();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Basket";
});

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcDiscount:DiscountUrl"]!);
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMessageBroker(builder.Configuration);

var app = builder.Build();

app.MapCarter();

app.UseExceptionHandler(options => {});

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();