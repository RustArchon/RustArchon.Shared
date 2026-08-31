// Copyright ©2026 Scott Blomfield

using System;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading a player's connect-to-disconnect session, including geolocation once (if ever)
/// resolved. Append-mostly on the API side - there is no create DTO, and the only updates are the
/// system itself closing the session out or filling in geolocation.
/// </summary>
public class PlayerSessionDto : EntityDto
{
    public Guid RustServerId { get; set; }
    public string SteamId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTimeOffset ConnectedAtUtc { get; set; }
    public DateTimeOffset? DisconnectedAtUtc { get; set; }
    public string? GeolocationProvider { get; set; }
    public string? GeolocationCountry { get; set; }
    public string? GeolocationCountryCode { get; set; }
    public bool? GeolocationIsVpn { get; set; }
    public bool? GeolocationIsProxy { get; set; }

    /// <summary>Network latency (ms) as of the most recent <c>playerlist</c> poll snapshot for this
    /// session - "last known ping," not updated after the session closes.</summary>
    public int? LastPing { get; set; }

    /// <summary>Rust's own anti-cheat violation level as of the most recent <c>playerlist</c> poll
    /// snapshot for this session.</summary>
    public decimal? LastViolationLevel { get; set; }

    public bool? SteamVacBanned { get; set; }
    public int? SteamNumberOfVacBans { get; set; }
    public int? SteamNumberOfGameBans { get; set; }

    /// <summary>Total playtime in Rust specifically, per Steam's own records, in minutes - <c>null</c>
    /// if never looked up, lookup failed, or the profile's game details aren't public.</summary>
    public int? SteamMinutesPlayedForever { get; set; }
}
