// Copyright ©2026 Scott Blomfield

using System;
using RustArchon.Messaging.Contracts;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One row of the Plugins tab - see <c>RustArchon.Messaging.Contracts.ServerPluginsCaptured</c> for
/// where these come from.
/// </summary>
/// <remarks>
/// Not an <c>EntityDto</c>, same reasoning as <see cref="ServerInfoSnapshotDto"/>: the tab lists rows by
/// name, nothing addresses one by identity.
/// </remarks>
public class ServerPluginDto
{
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public ServerModFramework Framework { get; set; }
    public DateTimeOffset CapturedAtUtc { get; set; }
}
