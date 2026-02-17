var builder = DistributedApplication.CreateBuilder(args);

// Configure dashboard (Aspire 13 requires explicit configuration)
builder.Configuration["ASPNETCORE_URLS"] = "http://localhost:15888";
builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] = "http://localhost:18889";
builder.Configuration["DASHBOARD__OTLP__AUTHMODE"] = "Unsecured";
builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

// PostgreSQL with Aspire auto-generated password
// Using Persistent lifetime to keep data between runs
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .AddDatabase("corelioDb");

// Redis cache (using persistent lifetime to keep cache between runs)
var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent);

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
