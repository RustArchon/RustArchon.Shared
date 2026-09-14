// Copyright ©2026 Scott Blomfield

using System;
using JumpStart.Api.DTOs;
using RustArchon.Messaging.Contracts;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading a captured WebRCON event (console output, chat, a kill-feed line, a command's
/// response, or a command that was sent). Append-only on the API side - there is no create/update DTO
/// for this entity.
/// </summary>
/// <remarks>
/// Only ever populated with rows the caller is actually allowed to see - both
/// <c>RustServersController.GetEvents</c> and <c>RconHub</c>'s live groups filter out
/// <see cref="Interactive"/> <c>false</c> rows before they ever reach this DTO for anyone but a site
/// admin who opted into the unfiltered view. This DTO carries the flag anyway (rather than omitting it
/// from the wire shape) so the Panel can render the "this row was hidden from ordinary users" badge
/// for that one audience without a second, shadow DTO type.
/// </remarks>
public class RconEventDto : EntityDto
{
    public Guid RustServerId { get; set; }
    public DateTimeOffset CapturedAtUtc { get; set; }
    public int Identifier { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Stacktrace { get; set; }
    public bool Interactive { get; set; }
    public RconEventDirection Direction { get; set; }
}
