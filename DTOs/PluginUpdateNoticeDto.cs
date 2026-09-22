// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// A plugin on a server that UpdateChecker (a third-party plugin the server runs) says has a newer version, with everything it
/// reported. Only notices that still hold are returned: a plugin that has since been updated or removed is left out.
/// </summary>
public class PluginUpdateNoticeDto
{
    public string Name { get; set; } = string.Empty;

    /// <summary>The version the server's plugin list says is installed now.</summary>
    public string InstalledVersion { get; set; } = string.Empty;

    /// <summary>The version UpdateChecker said was installed when it reported (usually the same).</summary>
    public string ReportedVersion { get; set; } = string.Empty;

    public string LatestVersion { get; set; } = string.Empty;

    /// <summary>
    /// The plugin's page on its marketplace, or empty. A page to visit, not a file to download. It came from another plugin, so the Api
    /// only passes on an absolute http or https address; anything else is empty.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// A direct download address for the newest version, or empty. Found by asking a public marketplace index, checked before it was kept
    /// (https, on the marketplace's own host) and again before it is sent here; empty for paid and login-only plugins, and until it has been
    /// looked up. Never fetched by the Panel - it is a link for a person to follow.
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;

    public string Marketplace { get; set; } = string.Empty;

    public DateTimeOffset FirstSeenUtc { get; set; }
    public DateTimeOffset LastSeenUtc { get; set; }
    public int TimesSeen { get; set; }
}
