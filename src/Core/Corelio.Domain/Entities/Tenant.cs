using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a tenant organization in the multi-tenant system.
/// </summary>
public class Tenant : AuditableEntity
{
    /// <summary>
    /// The display name of the tenant organization.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The legal business name for invoicing.
    /// </summary>
    public string LegalName { get; set; } = string.Empty;

    /// <summary>
    /// RFC (Registro Federal de Contribuyentes) - Mexican tax ID.
    /// Format: 3-4 letters + 6 digits + 3 alphanumeric.
    /// </summary>
    public string Rfc { get; set; } = string.Empty;

    /// <summary>
    /// Subdomain for tenant access (e.g., 'ferreteria-lopez' for ferreteria-lopez.corelio.com.mx).
    /// </summary>
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>
    /// Optional custom domain (e.g., 'ferreteria-lopez.com').
    /// </summary>
    public string? CustomDomain { get; set; }

    /// <summary>
    /// The subscription plan for this tenant.
    /// </summary>
    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Basic;

    /// <summary>
    /// When the current subscription period started.
    /// </summary>
    public DateTime SubscriptionStartsAt { get; set; }

    /// <summary>
    /// When the current subscription period ends (null for unlimited).
    /// </summary>
    public DateTime? SubscriptionEndsAt { get; set; }

    /// <summary>
    /// Maximum number of users allowed for this tenant.
    /// </summary>
    public int MaxUsers { get; set; } = 5;

    /// <summary>
    /// Maximum number of products allowed for this tenant.
    /// </summary>
    public int MaxProducts { get; set; } = 1000;

    /// <summary>
    /// Maximum number of sales per month allowed for this tenant.
    /// </summary>
    public int MaxSalesPerMonth { get; set; } = 5000;

    /// <summary>
    /// Whether the tenant is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the tenant is in trial mode.
    /// </summary>
    public bool IsTrial { get; set; } = false;

    /// <summary>
    /// When the trial period ends.
    /// </summary>
    public DateTime? TrialEndsAt { get; set; }

    // Navigation properties
    /// <summary>
    /// The tenant configuration settings.
    /// </summary>
    public TenantConfiguration? Configuration { get; set; }

    /// <summary>
    /// Users belonging to this tenant.
    /// </summary>
    public ICollection<User> Users { get; set; } = [];

    /// <summary>
    /// Custom roles created by this tenant.
    /// </summary>
    public ICollection<Role> Roles { get; set; } = [];
}
