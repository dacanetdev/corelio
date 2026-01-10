using Corelio.Infrastructure;
using Corelio.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (telemetry, health checks, service discovery)
builder.AddServiceDefaults();

// Add Infrastructure services (DbContext, interceptors, providers)
builder.AddInfrastructureServices();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map Aspire default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Placeholder endpoint - will be replaced with MapAllEndpoints()
app.MapGet("/", () => "Corelio API v1.0");

app.Run();
