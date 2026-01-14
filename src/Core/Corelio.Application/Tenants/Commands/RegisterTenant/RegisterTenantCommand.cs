using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Tenants.Commands.RegisterTenant;

/// <summary>
/// Command to register a new tenant with an owner user.
/// </summary>
/// <param name="TenantName">The business name of the tenant.</param>
/// <param name="RFC">The tenant's RFC (Mexican tax ID).</param>
/// <param name="Subdomain">The unique subdomain for the tenant (e.g., "ferreteria-lopez").</param>
/// <param name="OwnerEmail">The email address for the owner user account.</param>
/// <param name="OwnerPassword">The password for the owner user account.</param>
/// <param name="OwnerFirstName">The first name of the owner.</param>
/// <param name="OwnerLastName">The last name of the owner.</param>
public record RegisterTenantCommand(
    string TenantName,
    string RFC,
    string Subdomain,
    string OwnerEmail,
    string OwnerPassword,
    string OwnerFirstName,
    string OwnerLastName) : IRequest<Result<Guid>>;
