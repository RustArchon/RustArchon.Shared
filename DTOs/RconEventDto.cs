// Copyright ©2026 Scott Blomfield

using System;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading a captured WebRCON event (console output, chat, a kill-feed line, or a command's
/// response). Append-only on the API side - there is no create/update DTO for this entity.
/// </summary>
public class RconEventDto : EntityDto
{
    public Guid RustServerId { get; set; }
    public DateTimeOffset CapturedAtUtc { get; set; }
    public int Identifier { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Stacktrace { get; set; }
}
