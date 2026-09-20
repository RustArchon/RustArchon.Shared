// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>One time the Panel asked a server to update the RustArchon plugin or its Updater, and how it turned out.</summary>
public class PluginUpdateAttemptDto
{
    /// <summary><c>main</c> (the RustArchon plugin) or <c>updater</c>.</summary>
    public string Kind { get; set; } = string.Empty;

    public string FromVersion { get; set; } = string.Empty;
    public string ToVersion { get; set; } = string.Empty;

    /// <summary><c>manual</c> (someone pressed the button) or <c>auto</c>.</summary>
    public string Trigger { get; set; } = string.Empty;

    /// <summary>
    /// <c>started</c> (asked, outcome not known yet), <c>succeeded</c> (the new version is installed), <c>failed</c> (asked, but the version never
    /// changed - the new one did not come up and was rolled back) or <c>refused</c> (the server turned the request down; see <see cref="Code"/>).
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>Why it was refused or failed, when it was; otherwise empty.</summary>
    public string Code { get; set; } = string.Empty;

    public DateTimeOffset StartedAtUtc { get; set; }
    public DateTimeOffset? ResolvedAtUtc { get; set; }
}
