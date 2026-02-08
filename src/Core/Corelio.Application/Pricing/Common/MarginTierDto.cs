namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing a margin tier definition.
/// </summary>
public record MarginTierDto(
    int TierNumber,
    string TierName,
    bool IsActive);
