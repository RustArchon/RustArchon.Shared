// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// What the optional RustArchon companion plugin last reported on a server - see
/// <c>RustArchon.Messaging.Contracts.ServerPluginHandshakeCaptured</c>.
/// </summary>
/// <remarks>
/// Not an <c>EntityDto</c>, same reasoning as <see cref="ServerPluginDto"/>: it is a per-server status, never
/// addressed by its own identity. <see cref="Capabilities"/> is the fail-closed gate: a Panel feature is
/// available only when its capability is listed here (and the plugin is still in the server's plugin list).
/// </remarks>
public class ServerPluginStatusDto
{
    public int ProtocolVersion { get; set; }
    public string PluginVersion { get; set; } = string.Empty;
    public string[] Capabilities { get; set; } = [];

    /// <summary>What the plugin reports its Recording switch is (not what was asked for).</summary>
    public bool RecordingEnabled { get; set; }

    /// <summary>What the plugin reports its Combat log switch is (not what was asked for).</summary>
    public bool CombatLogEnabled { get; set; }

    /// <summary>False when the plugin could not save its settings, so they revert on its next reload.</summary>
    public bool SettingsPersisted { get; set; }

    /// <summary>
    /// The plugin's own check of its file against the signature a Panel put on it: <c>valid</c>, <c>invalid</c>,
    /// <c>unsigned</c>, <c>unlocated</c>, <c>error</c>, or <c>unknown</c> (a build too old to report it).
    /// </summary>
    public string SigningState { get; set; } = "unknown";

    /// <summary>The fingerprint of the key the installed plugin trusts; empty when it has none.</summary>
    public string SigningKeyFingerprint { get; set; } = string.Empty;

    /// <summary>
    /// Whether the key the installed plugin trusts is <em>this</em> Panel's: <c>true</c> when the fingerprints match
    /// (this Panel can sign updates that plugin will accept), <c>false</c> when they differ (it was installed from a
    /// different Panel, and will refuse this one's updates), <c>null</c> when it cannot be told (the plugin trusts no
    /// key, or this Panel has not made its key yet).
    /// </summary>
    public bool? SigningKeyMatchesThisPanel { get; set; }

    /// <summary>
    /// Where the key the installed plugin trusts stands in this Panel's key history: <c>active</c> (the current key),
    /// <c>retired</c> (an older one; an update moves the plugin to the current key), <c>revoked</c> (it can only be
    /// fixed by downloading the plugin by hand), or empty when it is not one of this Panel's keys (or is unknown).
    /// </summary>
    public string SigningKeyState { get; set; } = string.Empty;

    /// <summary>The version of the plugin this Panel currently serves - what an update would install. <c>null</c> if unknown.</summary>
    public string? LatestPluginVersion { get; set; }

    /// <summary>Whether <see cref="LatestPluginVersion"/> is strictly newer than the installed <see cref="PluginVersion"/>.</summary>
    public bool UpdateAvailable { get; set; }

    /// <summary>Whether the separate Updater plugin is in this server's plugin list. Without it the Panel cannot update the plugin.</summary>
    public bool UpdaterInstalled { get; set; }

    /// <summary>The Updater's installed version (from the server's plugin list), or <c>null</c> if it is not installed or has none.</summary>
    public string? UpdaterVersion { get; set; }

    /// <summary>The Updater version this Panel serves. The Updater is only ever replaced by hand, so a newer one is a prompt to download it.</summary>
    public string? LatestUpdaterVersion { get; set; }

    /// <summary>Whether <see cref="LatestUpdaterVersion"/> is newer than the installed <see cref="UpdaterVersion"/>.</summary>
    public bool UpdaterUpdateAvailable { get; set; }

    public DateTimeOffset CapturedAtUtc { get; set; }
}
