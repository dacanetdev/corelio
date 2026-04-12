using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Queries.GetCfdiSettings;

/// <summary>
/// Query to get the current tenant's CFDI issuer settings.
/// </summary>
public record GetCfdiSettingsQuery : IRequest<Result<CfdiSettingsDto>>;
