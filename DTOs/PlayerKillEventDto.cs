// Copyright ©2026 Scott Blomfield

using System;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading a captured (heuristically-parsed) kill event. Append-only on the API side - there
/// is no create/update DTO for this entity.
/// </summary>
public class PlayerKillEventDto : EntityDto
{
    public Guid RustServerId { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; }
    public string VictimName { get; set; } = string.Empty;
    public string? VictimSteamId { get; set; }
    public string? KillerName { get; set; }
    public string? KillerSteamId { get; set; }
    public string? Weapon { get; set; }
    public string RawMessage { get; set; } = string.Empty;
}
