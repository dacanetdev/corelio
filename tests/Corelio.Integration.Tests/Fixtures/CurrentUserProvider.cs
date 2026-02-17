using Corelio.Domain.Common.Interfaces;

namespace Corelio.Integration.Tests.Fixtures;

/// <summary>
/// Simple current user provider implementation for integration tests.
/// Returns null for UserId (no authenticated user in integration tests).
/// </summary>
public class CurrentUserProvider : ICurrentUserProvider
{
    public Guid? UserId => null;
}
