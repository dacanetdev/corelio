namespace Corelio.Integration.Tests.Fixtures;

/// <summary>
/// xUnit collection definition for PostgreSQL integration tests.
/// Ensures the PostgreSQL container is shared across all test classes in the collection.
/// </summary>
[CollectionDefinition("PostgreSQL")]
public class PostgreSqlCollection : ICollectionFixture<PostgreSqlTestContainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
