using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking system changes.
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// The tenant this audit log belongs to (null for system-level events).
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// The type of event (create, update, delete, login, etc.).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// The type of entity that was affected.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the affected entity.
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// The name/identifier of the affected entity for display.
    /// </summary>
    public string? EntityName { get; set; }

    /// <summary>
    /// The user who performed the action.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The user's email for historical reference.
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// The IP address of the user.
    /// </summary>
    public string? UserIp { get; set; }

    /// <summary>
    /// The user agent string.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// The old values (JSON) before the change.
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// The new values (JSON) after the change.
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// List of field names that changed.
    /// </summary>
    public string[]? ChangedFields { get; set; }

    /// <summary>
    /// HTTP method of the request.
    /// </summary>
    public string? RequestMethod { get; set; }

    /// <summary>
    /// Path of the request.
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// Unique request identifier.
    /// </summary>
    public Guid? RequestId { get; set; }

    /// <summary>
    /// Whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Event types for audit logging.
/// </summary>
public static class AuditEventTypes
{
    public const string Create = "create";
    public const string Update = "update";
    public const string Delete = "delete";
    public const string Login = "login";
    public const string Logout = "logout";
    public const string FailedLogin = "failed_login";
    public const string Export = "export";
    public const string Import = "import";
    public const string ConfigChange = "config_change";
    public const string PermissionChange = "permission_change";
    public const string CfdiStamp = "cfdi_stamp";
    public const string CfdiCancel = "cfdi_cancel";
}
