// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;
using JumpStart.Api.DTOs;
using RustArchon.Messaging.Contracts;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading a registered Rust server. Deliberately excludes the RCON password - it is
/// encrypted at rest and never returned by the API. See <see cref="CreateRustServerDto"/> and
/// <see cref="UpdateRustServerDto"/> for how it is written. Also excludes <c>AssignedWorkerId</c>/
/// <c>LastHeartbeatUtc</c> - internal worker-ownership plumbing, not something the UI needs.
/// </summary>
public class RustServerDto : AuditableEntityDto
{
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public RconConnectionStatus ConnectionStatus { get; set; }
    public string? ConnectionStatusDetail { get; set; }
    public DateTimeOffset? ConnectionStatusChangedAtUtc { get; set; }

    /// <summary>Whether a Steam Web API key is currently configured - never the key itself.</summary>
    public bool HasSteamApiKey { get; set; }

    public GeolocationProviderKind GeolocationProvider { get; set; }

    /// <summary>Whether an API key is currently configured for <see cref="GeolocationProvider"/> -
    /// never the key itself.</summary>
    public bool HasGeolocationApiKey { get; set; }

    /// <summary>When the Add Server wizard was finished for this server; null while it has not been (the servers list then offers "Finish setup").</summary>
    public DateTimeOffset? SetupCompletedAtUtc { get; set; }

    /// <summary>The saved (desired) state of the RustArchon plugin's Recording switch. Meaningful only on a
    /// server that has the plugin; see <see cref="ServerPluginStatusDto"/> for what the plugin reports.</summary>
    public bool PluginRecordingEnabled { get; set; }

    /// <summary>The saved (desired) state of the RustArchon plugin's Combat log switch.</summary>
    public bool PluginCombatLogEnabled { get; set; }

    /// <summary>Whether an admin may update the RustArchon plugin on this server from the Panel. Off by default.</summary>
    public bool PluginUpdatesEnabled { get; set; }

    /// <summary>
    /// Whether the Panel updates the plugin (and the Updater) on this server by itself when a newer version is being served. Off by
    /// default, and only ever on while <see cref="PluginUpdatesEnabled"/> is.
    /// </summary>
    public bool PluginAutoUpdateEnabled { get; set; }
}

/// <summary>
/// DTO for registering a new Rust server.
/// </summary>
public class CreateRustServerDto : ICreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 28016;

    /// <summary>
    /// The RCON password, in plaintext, over the wire only. Encrypted immediately on arrival at
    /// the API before it is ever persisted.
    /// </summary>
    [Required]
    public string RconPassword { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Optional Steam Web API key, in plaintext, over the wire only - encrypted immediately on
    /// arrival, same treatment as <see cref="RconPassword"/>. Powers VAC/game-ban status and
    /// hours-on-record for players on this server. Leave unset if you don't have one yet; it can be
    /// added later via update.
    /// </summary>
    public string? SteamApiKey { get; set; }

    /// <summary>Which geolocation/VPN-detection provider to use for players connecting to this
    /// server. <see cref="GeolocationProviderKind.None"/> (the default) skips lookups entirely.</summary>
    public GeolocationProviderKind GeolocationProvider { get; set; } = GeolocationProviderKind.None;

    /// <summary>
    /// The API key for <see cref="GeolocationProvider"/>, in plaintext, over the wire only -
    /// encrypted immediately on arrival, same treatment as <see cref="RconPassword"/>. Required for
    /// lookups to actually run whenever <see cref="GeolocationProvider"/> isn't
    /// <see cref="GeolocationProviderKind.None"/>.
    /// </summary>
    public string? GeolocationApiKey { get; set; }
}

/// <summary>
/// DTO for updating an existing Rust server.
/// </summary>
public class UpdateRustServerDto : IUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; }

    /// <summary>
    /// The new RCON password, in plaintext. Leave <c>null</c> or empty to keep the existing
    /// password unchanged.
    /// </summary>
    public string? RconPassword { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>The new Steam Web API key, in plaintext. Leave <c>null</c> or empty to keep the
    /// existing key unchanged - there is no way to clear a previously-set key via update other than
    /// replacing it with a new one.</summary>
    public string? SteamApiKey { get; set; }

    public GeolocationProviderKind GeolocationProvider { get; set; }

    /// <summary>The new geolocation provider's API key, in plaintext. Leave <c>null</c> or empty to
    /// keep the existing key unchanged.</summary>
    public string? GeolocationApiKey { get; set; }
}

/// <summary>
/// The RustArchon plugin's two switches for one server, saved by <c>PUT plugin-settings</c>. Deliberately
/// <em>not</em> part of <see cref="UpdateRustServerDto"/>: that is a full-record PUT, and if it carried these an
/// ordinary edit (a rename, a new password) from a client that does not send them would silently reset a switch
/// someone turned off. Both values are required, so an omitted one is a 400 rather than an accidental
/// <c>false</c>.
/// </summary>
public class UpdateServerPluginSettingsDto
{
    [Required]
    public bool? RecordingEnabled { get; set; }

    [Required]
    public bool? CombatLogEnabled { get; set; }

    /// <summary>
    /// Whether an admin may update the plugin from the Panel on this server. Required like the other two, for the same
    /// reason: an omitted value must be a 400, not a silent change - and here the safe reading of "missing" would be
    /// off, which is exactly what a client that does not know about this setting must not be able to flip by accident.
    /// </summary>
    [Required]
    public bool? UpdatesEnabled { get; set; }

    /// <summary>
    /// Whether the Panel updates this server's plugin by itself. <b>Optional, unlike the three above</b>: a missing value means "leave it as it
    /// is", so a client that does not know about this setting can neither turn it on nor off by accident. Turning
    /// <see cref="UpdatesEnabled"/> off turns this off too, whatever is sent.
    /// </summary>
    public bool? AutoUpdateEnabled { get; set; }
}

/// <summary>
/// The outcome of asking the Panel to update the RustArchon plugin on one server. A refusal is an ordinary result,
/// not an error: <see cref="Code"/> says why, so the Panel can tell the admin what to fix.
/// </summary>
public class PluginUpdateResultDto
{
    /// <summary>True when the Updater accepted the request and began downloading.</summary>
    public bool Started { get; set; }

    /// <summary>
    /// <c>started</c>, or the reason it did not: <c>updates_disabled</c>, <c>server_disabled</c>, <c>no_handshake</c>,
    /// <c>not_signed_by_this_panel</c>, <c>updater_missing</c>, <c>up_to_date</c>, <c>panel_url_invalid</c>,
    /// <c>not_connected</c>, <c>timeout</c>, or a code the Updater itself returned (<c>busy</c>, <c>not_newer</c>, ...).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
    public string? FromVersion { get; set; }
    public string? ToVersion { get; set; }
}
