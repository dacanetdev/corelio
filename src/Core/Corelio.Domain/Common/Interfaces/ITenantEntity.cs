namespace Corelio.Domain.Common.Interfaces;

/// <summary>
/// Interface for entities that belong to a specific tenant.
/// All business entities must implement this interface for multi-tenancy support.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// The tenant identifier that owns this entity.
    /// </summary>
    Guid TenantId { get; set; }
}
