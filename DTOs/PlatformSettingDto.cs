// Copyright ©2026 Scott Blomfield

using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Mirrors <c>RustArchon.Api.Data.PlatformSettingValueType</c> - duplicated here rather than shared
/// via a project reference, the same way DTOs never reference API-side entity types directly. Values 
/// are hard coded because they are persisted in the database, so changing them would break existing data.
/// See the Api-side enum's remarks for more details.
/// </summary>
public enum PlatformSettingValueType
{
    Boolean = 0,
    String = 1,
    Integer = 2,

    /// <summary>Value is either empty (unset) or a Plan's Id, as a Guid string - see the Api-side
    /// entity enum's remarks.</summary>
    PlanReference = 3,

    /// <summary>
    /// Encrypted at rest; <see cref="PlatformSettingDto.Value"/> is always blank for one of these - see
    /// <see cref="PlatformSettingDto.HasValue"/> and the Api-side entity enum's remarks.
    /// </summary>
    Secret = 4,

    /// <summary>
    /// Value is one of a fixed, code-defined set of options - see <see cref="PlatformSettingDto.Options"/>.
    /// Generic on purpose, unlike a dedicated enum member per picker would be: the set of choices lives
    /// on the setting itself, not in this type, so a new single-select setting never needs a new
    /// <see cref="PlatformSettingValueType"/> member the way <c>EmailServiceProvider</c> briefly did.
    /// </summary>
    Choice = 5,
}

/// <summary>
/// DTO for reading a platform-wide setting. There is no create DTO - settings are seeded by
/// <c>PlatformSettingsRegistry</c>, never created ad hoc through the admin UI; see
/// <c>PlatformSettingsController</c>'s remarks.
/// </summary>
public class PlatformSettingDto : EntityDto
{
    public string Key { get; set; } = string.Empty;

    /// <summary>Which section of the admin settings page this belongs under (e.g. "Email") - see the
    /// Api-side entity's remarks.</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Sort position within <see cref="Category"/>, ascending - see the Api-side entity's
    /// remarks for why this replaced sorting by <see cref="DisplayName"/>.</summary>
    public int Order { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PlatformSettingValueType ValueType { get; set; }

    /// <summary>
    /// For <see cref="PlatformSettingValueType.Choice"/>, the allowed values, comma-separated (e.g.
    /// <c>"Smtp,Resend,SendGrid"</c>) - null for every other <see cref="ValueType"/>.
    /// </summary>
    public string? Options { get; set; }

    /// <summary>
    /// The <see cref="Key"/> of another setting that controls whether this one is shown at all - null
    /// if this setting is always shown. See <see cref="VisibleWhenValue"/>/<see cref="VisibleWhenNegate"/>
    /// for the actual condition, and the Api-side entity's remarks for the full mechanism.
    /// </summary>
    public string? VisibleWhenKey { get; set; }

    /// <summary>The value <see cref="VisibleWhenKey"/>'s setting must currently hold (or, if
    /// <see cref="VisibleWhenNegate"/>, must <em>not</em> hold) for this setting to be shown.</summary>
    public string? VisibleWhenValue { get; set; }

    /// <summary>Inverts <see cref="VisibleWhenValue"/>'s comparison - "show me when it's anything else."</summary>
    public bool VisibleWhenNegate { get; set; }

    /// <summary>
    /// The setting's current value - except for <see cref="PlatformSettingValueType.Secret"/>, which is
    /// always blank here regardless of what's actually stored. A stored secret is never sent to a
    /// browser; see <see cref="HasValue"/> for the only thing the admin UI is told about one.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// For a <see cref="PlatformSettingValueType.Secret"/> setting, whether a value is currently
    /// stored - lets the admin UI show "configured" vs. "not set" without ever seeing the value itself.
    /// Always <c>true</c> for every other <see cref="ValueType"/>, where <see cref="Value"/> already
    /// carries that information directly.
    /// </summary>
    public bool HasValue { get; set; } = true;
}

/// <summary>
/// Request body for updating a setting's value. Only <see cref="Value"/> is ever editable - the
/// setting's <c>Key</c>/<c>DisplayName</c>/<c>Description</c>/<c>ValueType</c> are fixed by whichever
/// feature registered it.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Value"/> is deliberately not <c>[Required]</c> - an empty string is itself a real,
/// storable value for most setting types (<c>DefaultPlanId</c>'s unset state, <c>EmailSmtpHost</c>'s
/// "not configured" state, ...), not the absence of one. Rejecting it here would make "clear this
/// setting back to unset" impossible through this endpoint.
/// </para>
/// <para>
/// For a <see cref="PlatformSettingValueType.Secret"/> setting specifically, there is no way to submit
/// "leave it unchanged" - the admin's browser never has the real value to send back in the first place
/// (see <c>PlatformSettingDto.HasValue</c>), so the Panel's own UI simply never calls this endpoint for
/// a secret field the admin didn't type a new value into. A request that does arrive with an empty
/// <see cref="Value"/> for a Secret setting is refused rather than silently accepted as either
/// "unchanged" or "cleared" - see <c>PlatformSettingsController.UpdateValue</c>.
/// </para>
/// </remarks>
public class UpdatePlatformSettingValueDto
{
    public string Value { get; set; } = string.Empty;
}
