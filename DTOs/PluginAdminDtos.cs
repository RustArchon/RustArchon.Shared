// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>One plugin signing key as the platform-admin page shows it. Never carries anything secret.</summary>
public class PluginKeyDto
{
    /// <summary>First 16 hex characters of SHA-256 over the public modulus.</summary>
    public string Fingerprint { get; set; } = string.Empty;

    /// <summary><c>active</c> (signs new files), <c>retired</c> (kept, can still sign a bridge) or <c>revoked</c> (never signs again).</summary>
    public string State { get; set; } = string.Empty;

    public DateTimeOffset? RetiredAtUtc { get; set; }
    public DateTimeOffset? RevokedAtUtc { get; set; }
    public string? RevokedReason { get; set; }

    /// <summary>
    /// How many servers' latest handshake named this key. Information for the decision only: an offline or lagging
    /// server reports nothing, so this never proves a key is unused.
    /// </summary>
    public int ServersReporting { get; set; }

    public DateTimeOffset? LastReportedUtc { get; set; }
}

public class RotatePluginKeyRequestDto
{
    [MaxLength(500)]
    public string? Note { get; set; }

    /// <summary>Revoke the outgoing key instead of just retiring it (a suspected compromise).</summary>
    public bool RevokeCurrent { get; set; }

    [MaxLength(500)]
    public string? RevokeReason { get; set; }
}

public class RevokePluginKeyRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>One uploaded plugin release.</summary>
public class PluginReleaseDto
{
    public Guid Id { get; set; }

    /// <summary><c>main</c> (RustArchon.cs) or <c>updater</c> (RustArchonUpdater.cs).</summary>
    public string Kind { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    /// <summary><c>draft</c>, <c>published</c> or <c>withdrawn</c>.</summary>
    public string State { get; set; } = string.Empty;

    /// <summary>Whether this is the release the Panel is serving right now.</summary>
    public bool IsServed { get; set; }

    public string Sha256 { get; set; } = string.Empty;
    public DateTimeOffset UploadedAtUtc { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTimeOffset? PublishedAtUtc { get; set; }
    public string? PublishedBy { get; set; }
    public DateTimeOffset? WithdrawnAtUtc { get; set; }
    public string? WithdrawnBy { get; set; }
    public string? WithdrawnReason { get; set; }
    public string? Notes { get; set; }
}

/// <summary>What the Panel is serving for one plugin file right now.</summary>
public class PluginServedDto
{
    public string Kind { get; set; } = string.Empty;

    /// <summary>The version served (the newest published release if newer than the embedded build, else the embedded one).</summary>
    public string? ServedVersion { get; set; }

    /// <summary>The version built into this Api, served when no published release is newer.</summary>
    public string? EmbeddedVersion { get; set; }

    /// <summary>Whether <see cref="ServedVersion"/> comes from an uploaded release rather than the embedded build.</summary>
    public bool FromRelease { get; set; }
}

public class PluginReleasesDto
{
    public PluginServedDto Main { get; set; } = new();
    public PluginServedDto Updater { get; set; } = new();
    public System.Collections.Generic.List<PluginReleaseDto> Releases { get; set; } = new();
}

public class WithdrawPluginReleaseRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>One line of the plugin admin audit log.</summary>
public class PluginAdminEventDto
{
    public DateTimeOffset AtUtc { get; set; }

    /// <summary><c>KeyRotated</c>, <c>KeyRevoked</c>, <c>ReleaseUploaded</c>, <c>ReleasePublished</c> or <c>ReleaseWithdrawn</c>.</summary>
    public string Kind { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public string? Detail { get; set; }
}
