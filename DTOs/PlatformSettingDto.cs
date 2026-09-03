// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Mirrors <c>RustArchon.Api.Data.PlatformSettingValueType</c> - duplicated here rather than shared
/// via a project reference, the same way DTOs never reference API-side entity types directly.
/// </summary>
public enum PlatformSettingValueType
{
    Boolean,
    String,
    Integer,

    /// <summary>Value is either empty (unset) or a Plan's Id, as a Guid string - see the Api-side
    /// entity enum's remarks.</summary>
    PlanReference
}

/// <summary>
/// DTO for reading a platform-wide setting. There is no create DTO - settings are seeded by
/// <c>PlatformSettingsRegistry</c>, never created ad hoc through the admin UI; see
/// <c>PlatformSettingsController</c>'s remarks.
/// </summary>
public class PlatformSettingDto : EntityDto
{
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PlatformSettingValueType ValueType { get; set; }
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Request body for updating a setting's value. Only <see cref="Value"/> is ever editable - the
/// setting's <c>Key</c>/<c>DisplayName</c>/<c>Description</c>/<c>ValueType</c> are fixed by whichever
/// feature registered it.
/// </summary>
public class UpdatePlatformSettingValueDto
{
    [Required]
    public string Value { get; set; } = string.Empty;
}
