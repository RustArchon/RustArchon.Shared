// Copyright ©2026 Scott Blomfield

using System;
using RustArchon.Messaging.Contracts;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One entry on the Logs tab - either a WebRCON connection-status transition (<see cref="Status"/>
/// set) or a worker-side diagnostic that isn't itself a connection-status change (<see cref="Status"/>
/// null) - a parse error, a poll failure, and the like - so a past failure is diagnosable from the
/// Panel itself instead of only from a worker instance's own process logs. See
/// <c>RustArchon.Api.Data.ConnectionLogEntry</c>'s remarks.
/// </summary>
/// <remarks>
/// Not an <c>EntityDto</c> - like <see cref="ServerInfoSnapshotDto"/>, this is listed by
/// <see cref="OccurredAtUtc"/>, not by row identity.
/// </remarks>
public class ConnectionLogEntryDto
{
    public ConnectionLogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public RconConnectionStatus? Status { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; }
}
