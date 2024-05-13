var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(assembly);
    configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")
                       ?? throw new InvalidOperationException("Connection string was not provided"));
}).UseLightweightSessions();

var app = builder.Build();

app.MapCarter();

app.UseExceptionHandler(options => { });

app.Run();