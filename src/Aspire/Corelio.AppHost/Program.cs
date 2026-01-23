var builder = DistributedApplication.CreateBuilder(args);

// Configure dashboard (Aspire 13 requires explicit configuration)
builder.Configuration["ASPNETCORE_URLS"] = "http://localhost:15888";
builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] = "http://localhost:18889";
builder.Configuration["DASHBOARD__OTLP__AUTHMODE"] = "Unsecured";
builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

// PostgreSQL with Aspire auto-generated password
// Testing without data volumes first to isolate password issue
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("corelioDb");

// Redis cache (testing without data volume)
var redis = builder.AddRedis("redis");

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
