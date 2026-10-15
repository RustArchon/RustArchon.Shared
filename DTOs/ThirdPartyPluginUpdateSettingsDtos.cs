// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Where one server stands on having updates to its <b>third-party</b> plugins (the ones UpdateChecker reports) applied automatically: the
/// three gates - the plan offers it, the server opted in, and it is outside its days-before-wipe window - and the numbers behind them.
/// Nothing here concerns RustArchon's own plugin, which has its own switches.
/// </summary>
public class ThirdPartyPluginUpdateSettingsDto
{
    /// <summary>The plan gate: whether the organization's plan offers the feature at all. When false the Panel does not offer the settings.</summary>
    public bool PlanOffers { get; set; }

    /// <summary>The server gate: whether this server has opted in. Off until someone turns it on.</summary>
    public bool Enabled { get; set; }

    /// <summary>The time gate: how many days before the monthly wipe updates are held back. 0 means never held.</summary>
    public int HoldDays { get; set; }

    /// <summary>The largest <see cref="HoldDays"/> the Api accepts.</summary>
    public int MaxHoldDays { get; set; }

    /// <summary>The next monthly wipe: the first Thursday of the month at 19:00 London time.</summary>
    public DateTimeOffset NextWipeUtc { get; set; }

    /// <summary>When the hold lifts, if the server is inside its window right now (regardless of the other gates); otherwise null.</summary>
    public DateTimeOffset? HeldUntilUtc { get; set; }

    /// <summary>
    /// The first gate that is closed, or <c>open</c>: <c>plan_does_not_offer</c>, <c>not_opted_in</c>, <c>held_for_wipe</c>. A code for the
    /// Panel to explain, not text to show.
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>The platform-wide switch for automatic third-party updates (a Platform Setting). When off, no server is updated automatically.</summary>
    public bool PlatformSwitchOn { get; set; } = true;

    /// <summary>
    /// Whether the automatic pass would start an update on this server right now: <c>eligible</c>, or the first thing stopping it -
    /// <c>platform_switch_off</c>, then the codes of <see cref="State"/>, then the plugin-side ones (<c>not_signed_by_this_panel</c>, <c>key_revoked</c>,
    /// <c>plugin_too_old</c>, <c>stale_status</c>, <c>no_handshake</c>, <c>server_disabled</c>). A code for the Panel to link a remedy to.
    /// </summary>
    public string AutoState { get; set; } = "eligible";

    /// <summary>A sentence saying what <see cref="AutoState"/> means and what to do about it. Empty when eligible.</summary>
    public string AutoStateMessage { get; set; } = string.Empty;

    /// <summary>When the automatic pass last started, or null if it has not run since the Api started.</summary>
    public DateTimeOffset? LastPassUtc { get; set; }

    /// <summary>When the automatic pass next runs, or null if not yet scheduled.</summary>
    public DateTimeOffset? NextPassUtc { get; set; }
}

/// <summary>
/// The third-party plugin update settings saved by <c>PUT third-party-update-settings</c>. Both values are required, so a client that does not
/// send one gets a 400 rather than an accidental change: an omitted opt-in must never read as "off" (silently withdrawing a choice) or "on".
/// Deliberately not part of <see cref="UpdateRustServerDto"/>, for the same reason the plugin switches are not: that is a full-record PUT.
/// </summary>
public class UpdateThirdPartyPluginUpdateSettingsDto
{
    [Required]
    public bool? Enabled { get; set; }

    /// <summary>Days before the monthly wipe to hold updates back; 0 means never (every update is taken right up to wipe day).</summary>
    [Required]
    [Range(0, 21)]
    public int? HoldDays { get; set; }
}
