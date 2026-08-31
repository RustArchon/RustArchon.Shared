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
