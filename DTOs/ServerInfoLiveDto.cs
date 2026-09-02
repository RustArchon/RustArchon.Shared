// Copyright ©2026 Scott Blomfield

namespace RustArchon.Shared.DTOs;

/// <summary>
/// The full <c>serverinfo</c> RCON response, for the Stats tab's live info panel - point-in-time
/// server metadata (hostname, map, version, uptime, ...) that's worth displaying but not worth
/// graphing over time. See <c>ServerInfoSnapshotDto</c> for the handful of fields that *are* graphed.
/// </summary>
/// <remarks>
/// Deserialized directly from <c>RconCommandResult.Message</c> on the Panel side (see
/// <c>ServerDetail.razor</c>'s <c>LoadLiveServerInfoAsync</c>) rather than round-tripped through a
/// dedicated Api endpoint - the Panel already has a generic "run this RCON command and get the raw
/// result" pathway (<c>IRustServerApiClient.SendCommandAsync</c>, the same one the Console tab uses),
/// and adding a second Api-side deserialization of this exact payload would mean either giving
/// RustArchon.Api a project reference to RustArchon.Rcon purely to reuse <c>ServerInfoParser</c>'s
/// custom date handling, or duplicating it - neither worth it for a page that already has the raw
/// JSON one hop away. <see cref="GameTime"/>/<see cref="SaveCreatedTime"/> are kept as plain strings
/// rather than <c>DateTime</c> for exactly that reason: Rust's <c>serverinfo</c> emits them in a
/// non-ISO format that needs a custom converter to parse (see
/// <c>RustArchon.Rcon.Converters.DateTimeConverter</c>), and this DTO has no need to parse them at
/// all - it only ever displays them verbatim.
/// </remarks>
public class ServerInfoLiveDto
{
    public string Hostname { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public int Players { get; set; }
    public int Queued { get; set; }
    public int Joining { get; set; }
    public int ReservedSlots { get; set; }
    public int EntityCount { get; set; }
    public string GameTime { get; set; } = string.Empty;
    public int Uptime { get; set; }
    public string Map { get; set; } = string.Empty;
    public decimal Framerate { get; set; }
    public int Memory { get; set; }
    public int MemoryUsageSystem { get; set; }
    public int Collections { get; set; }
    public int NetworkIn { get; set; }
    public int NetworkOut { get; set; }
    public bool Restarting { get; set; }
    public string SaveCreatedTime { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Protocol { get; set; } = string.Empty;
}
