// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// What kind of report a player filed from Rust's F7 menu. The numbering is Rust's own
/// (https://wiki.facepunch.com/rust/receiving-reports) - it is what arrives in the <c>Type</c> field, so it must not be
/// renumbered.
/// </summary>
public enum ServerReportType
{
    General = 0,
    Bug = 1,
    Cheat = 2,
    Abuse = 3,
    Idea = 4
}

/// <summary>Where an admin has got to with a report.</summary>
public enum ServerReportStatus
{
    New = 0,
    Reviewing = 1,
    Actioned = 2,
    Dismissed = 3
}

/// <summary>
/// Which delivery route(s) contributed to a report. A flags value because one report can arrive by both routes and is then
/// merged into a single row (ADR-0003).
/// </summary>
[Flags]
public enum ServerReportSource
{
    None = 0,

    /// <summary>The game server posted it to <c>server.reportsServerEndpoint</c>.</summary>
    Native = 1,

    /// <summary>The RustArchon plugin reported it.</summary>
    Plugin = 2
}

/// <summary>DTO for reading a report.</summary>
public class ServerReportDto : EntityDto
{
    public Guid RustServerId { get; set; }
    public DateTimeOffset ReceivedAtUtc { get; set; }

    /// <summary>
    /// A flags value, so a report that arrived by both routes goes over the wire as <c>"Native, Plugin"</c>. The converter is named on
    /// the property because that is the only place that beats the options-level one Refit installs, which reads a single name and
    /// throws on a combination - which took the whole inbox down the first time a merged report existed.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ServerReportSource Source { get; set; }
    public ServerReportType Type { get; set; }
    public ServerReportStatus Status { get; set; }

    public string? ReporterSteamId { get; set; }
    public string? ReporterName { get; set; }
    public string? TargetSteamId { get; set; }
    public string? TargetName { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>The reporter's in-game position when they filed it, as the game formatted it.</summary>
    public string? Position { get; set; }
    public int? MinutesPlayed { get; set; }

    /// <summary>Whether a screenshot was attached. The picture itself is fetched separately, only when asked for.</summary>
    public bool HasScreenshot { get; set; }

    /// <summary>
    /// Extra detail the plugin supplied, as raw JSON, or <c>null</c> when it did not report this one. Untrusted: it came from a
    /// plugin on someone else's server and is shown, never acted on.
    /// </summary>
    public string? PluginDetailJson { get; set; }

    /// <summary>True when the payload could not be understood; the raw text was still kept.</summary>
    public bool ParseFailed { get; set; }

    public DateTimeOffset? ReviewedAtUtc { get; set; }

    /// <summary>The member of the organization this report is assigned to, or <c>null</c> when nobody has picked it up.</summary>
    public Guid? AssignedToUserId { get; set; }
}

/// <summary>Body for changing a report's status.</summary>
public class UpdateServerReportStatusDto
{
    public ServerReportStatus Status { get; set; }
}

/// <summary>Body for assigning a report. <c>null</c> takes the assignment back.</summary>
public class AssignServerReportDto
{
    public Guid? AssignedToUserId { get; set; }
}

/// <summary>An organization's internal note on a report. Never shown to the reporter - nothing in this system talks back to them.</summary>
public class ServerReportNoteDto
{
    public Guid Id { get; set; }

    /// <summary>The member who wrote it. The Api holds no names; the Panel resolves this from its own account store.</summary>
    public Guid AuthorUserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }
}

/// <summary>Body for adding a note to a report.</summary>
public class SaveServerReportNoteDto
{
    public string Content { get; set; } = string.Empty;
}

/// <summary>How many reports are waiting, for the tab's badge.</summary>
public class ServerReportCountDto
{
    public int New { get; set; }
}

/// <summary>
/// The address an admin pastes into <c>server.reportsServerEndpoint</c> so their game server forwards F7 reports here.
/// </summary>
/// <remarks>
/// The address carries the secret (ADR-0001), so it is only ever returned to someone allowed to manage report forwarding, and it
/// is never logged.
/// </remarks>
public class ReportForwardingDto
{
    /// <summary>The full address, secret included.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>The ready-to-paste console line.</summary>
    public string Command { get; set; } = string.Empty;

    /// <summary>Whether a check has ever confirmed the game server is set to this address.</summary>
    public bool Verified { get; set; }

    public DateTimeOffset? VerifiedAtUtc { get; set; }

    /// <summary>When the last report arrived by the native route, so the card can show forwarding is really working.</summary>
    public DateTimeOffset? LastReportReceivedAtUtc { get; set; }
}

/// <summary>What reading the convar back from the game server showed.</summary>
public enum ReportForwardingVerdict
{
    /// <summary>The game server is set to exactly this address.</summary>
    Matches = 0,

    /// <summary>It is set, but to something else.</summary>
    Mismatch = 1,

    /// <summary>It is not set.</summary>
    NotSet = 2,

    /// <summary>The server answered, but not in a form we could read.</summary>
    Unreadable = 3,

    /// <summary>We could not reach the server's console to ask (not connected, or it did not answer in time).</summary>
    Unavailable = 4
}

/// <summary>The result of asking the game server what it is set to.</summary>
public class VerifyReportForwardingResultDto
{
    public ReportForwardingVerdict Verdict { get; set; }

    /// <summary>The server's current value with the secret removed, so the admin can see where it points. Never the secret.</summary>
    public string? ObservedRedacted { get; set; }
}

/// <summary>Which third-party credential a verify request is about.</summary>
public enum IntegrationKeyKind
{
    SteamWebApi = 0,
    Geolocation = 1
}

/// <summary>Body for checking a key with its provider before it is saved.</summary>
public class VerifyIntegrationKeyRequest
{
    public IntegrationKeyKind Kind { get; set; }

    /// <summary>For <see cref="IntegrationKeyKind.Geolocation"/>: which provider the key belongs to.</summary>
    public GeolocationProviderKind Provider { get; set; }

    /// <summary>The key to check. Not stored by verifying.</summary>
    public string Key { get; set; } = string.Empty;
}

/// <summary>What a provider said about a key.</summary>
public enum IntegrationKeyVerdict
{
    /// <summary>The provider positively accepted the key.</summary>
    Valid = 0,

    /// <summary>The provider positively rejected the key.</summary>
    InvalidKey = 1,

    /// <summary>The provider is limiting requests; try again later.</summary>
    RateLimited = 2,

    /// <summary>The provider could not be reached.</summary>
    Unreachable = 3,

    /// <summary>The provider answered in a way we could not interpret. Never treated as valid.</summary>
    Unknown = 4
}

/// <summary>Result of <see cref="VerifyIntegrationKeyRequest"/>.</summary>
public class VerifyIntegrationKeyResultDto
{
    public IntegrationKeyVerdict Verdict { get; set; }
}

/// <summary>A page of reports.</summary>
public class ServerReportListDto
{
    public List<ServerReportDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
