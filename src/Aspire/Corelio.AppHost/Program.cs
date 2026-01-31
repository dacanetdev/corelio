var builder = DistributedApplication.CreateBuilder(args);

// Configure dashboard (Aspire 13 requires explicit configuration)
builder.Configuration["ASPNETCORE_URLS"] = "http://localhost:15888";
builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] = "http://localhost:18889";
builder.Configuration["DASHBOARD__OTLP__AUTHMODE"] = "Unsecured";
builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

// PostgreSQL with Aspire auto-generated password
// Using WithLifetime to ensure fresh database on each restart during development
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Session)
    .WithPgAdmin()
    .AddDatabase("corelioDb");

// Redis cache (using session lifetime for fresh start)
var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Session);

var api = builder.AddProject<Projects.Corelio_WebAPI>("api")
    .WithReference(postgres)
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(redis);

builder.AddProject<Projects.Corelio_BlazorApp>("blazor")
    .WithReference(api)
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();
