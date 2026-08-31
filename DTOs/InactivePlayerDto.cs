// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One row of the "inactive players" summary - every distinct player who has connected to a server
/// at some point but isn't currently connected, with their most recent session's details plus their
/// aggregate playtime across every session on this server.
/// </summary>
/// <remarks>
/// Not an <c>EntityDto</c> - this is a computed/aggregated view over <c>PlayerSession</c> history,
/// not a persisted entity in its own right, so it carries no <c>Id</c>/audit fields. <see cref="SteamId"/>
/// is what actually identifies a row.
/// </remarks>
public class InactivePlayerDto
{
    public string SteamId { get; set; } = string.Empty;

    /// <summary>Display name as of their most recent session.</summary>
    public string DisplayName { get; set; } = string.Empty;

    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public bool? IsVpn { get; set; }

    public bool? VacBanned { get; set; }
    public int? NumberOfVacBans { get; set; }
    public int? NumberOfGameBans { get; set; }

    /// <summary>Total playtime in Rust specifically, per Steam's own records, in minutes - <c>null</c>
    /// if never looked up, lookup failed, or the profile's game details aren't public.</summary>
    public int? SteamMinutesPlayedForever { get; set; }

    /// <summary>Total time connected to this server, summed across every session on record.</summary>
    public TimeSpan HoursOnServer { get; set; }

    /// <summary>Network latency (ms) as of their most recent session's last poll snapshot.</summary>
    public int? LastPing { get; set; }

    /// <summary>Rust's own anti-cheat violation level as of their most recent session's last poll
    /// snapshot.</summary>
    public decimal? LastViolationLevel { get; set; }

    public string IpAddress { get; set; } = string.Empty;

    public DateTimeOffset LastConnectedAtUtc { get; set; }
    public DateTimeOffset? LastDisconnectedAtUtc { get; set; }

    /// <summary>How long their most recent session lasted. Always populated for a row in this list -
    /// being here at all means their latest session has already closed.</summary>
    public TimeSpan LastConnectionDuration { get; set; }
}
