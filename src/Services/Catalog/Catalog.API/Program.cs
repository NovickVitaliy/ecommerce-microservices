var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database") 
                   ?? throw new InvalidOperationException("Connection string was not provided"));
}).UseLightweightSessions();

var app = builder.Build();

app.MapCarter();

app.Run();