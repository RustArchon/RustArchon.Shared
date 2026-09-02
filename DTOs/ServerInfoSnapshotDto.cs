// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One point on the Stats tab's graphs - see <c>RustArchon.Messaging.Contracts.ServerInfoSnapshotCaptured</c>'s
/// remarks for why only these fields are captured at all.
/// </summary>
/// <remarks>
/// Not an <c>EntityDto</c> - a chart plots by <see cref="CapturedAtUtc"/>, not by row identity, so
/// there's no need to carry an <c>Id</c> across the wire.
/// </remarks>
public class ServerInfoSnapshotDto
{
    public int Players { get; set; }
    public int MaxPlayers { get; set; }
    public int NetworkIn { get; set; }
    public int NetworkOut { get; set; }
    public int Memory { get; set; }
    public DateTimeOffset CapturedAtUtc { get; set; }
}
