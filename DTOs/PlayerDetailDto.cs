// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Everything known about one player on one server - the summary shown at the top of the player
/// detail page. Their session history and kill involvement are separate, paginated calls (see
/// <c>IRustServerApiClient.GetPlayerSessionsAsync</c>/<c>GetPlayerKillsAsync</c>), not part of this DTO.
/// </summary>
/// <remarks>
/// Not an <c>EntityDto</c> - a computed/aggregated view over <c>PlayerSession</c> history, the same as
/// <see cref="InactivePlayerDto"/>, just scoped to one already-known player instead of listing many.
/// </remarks>
public class PlayerDetailDto
{
    public string SteamId { get; set; } = string.Empty;

    /// <summary>Display name as of their most recent session.</summary>
    public string DisplayName { get; set; } = string.Empty;

    public bool IsCurrentlyConnected { get; set; }

    /// <summary>Only populated while <see cref="IsCurrentlyConnected"/> is true.</summary>
    public DateTimeOffset? CurrentSessionConnectedAtUtc { get; set; }

    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public bool? IsVpn { get; set; }

    public bool? VacBanned { get; set; }
    public int? NumberOfVacBans { get; set; }
    public int? NumberOfGameBans { get; set; }

    /// <summary>Total playtime in Rust specifically, per Steam's own records, in minutes - <c>null</c>
    /// if never looked up, lookup failed, or the profile's game details aren't public.</summary>
    public int? SteamMinutesPlayedForever { get; set; }

    /// <summary>Total time connected to this server, summed across every session on record (including
    /// the currently-open one, if any, counted up to now).</summary>
    public TimeSpan HoursOnServer { get; set; }

    public int SessionCount { get; set; }
    public DateTimeOffset FirstConnectedAtUtc { get; set; }
    public DateTimeOffset LastConnectedAtUtc { get; set; }
}
