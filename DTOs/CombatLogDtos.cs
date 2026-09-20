// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>One recorded hit or death from the RustArchon plugin's combat log.</summary>
public class CombatEventDto
{
    /// <summary>The plugin's sequence number for this event within one run of the plugin. Not unique across reloads.</summary>
    public long Sequence { get; set; }

    public DateTimeOffset OccurredAtUtc { get; set; }

    /// <summary><c>hit</c> or <c>death</c>.</summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>The attacker's SteamID when it is a player, otherwise the entity's prefab name (for example <c>bear</c>); empty if unknown.</summary>
    public string AttackerId { get; set; } = string.Empty;
    public string AttackerName { get; set; } = string.Empty;
    public bool AttackerIsPlayer { get; set; }

    public string VictimId { get; set; } = string.Empty;
    public string VictimName { get; set; } = string.Empty;
    public bool VictimIsPlayer { get; set; }

    public string Weapon { get; set; } = string.Empty;
    public double Damage { get; set; }
    public string DamageType { get; set; } = string.Empty;
    public bool Headshot { get; set; }

    /// <summary>Metres between attacker and victim, when the attacker's position was known.</summary>
    public double? Distance { get; set; }

    public double[]? AttackerPosition { get; set; }
    public double[]? VictimPosition { get; set; }
}

/// <summary>A page of combat events, newest first.</summary>
public class CombatLogDto
{
    public List<CombatEventDto> Events { get; set; } = new();

    /// <summary>Whether older events matching the same filter exist beyond this page.</summary>
    public bool HasMore { get; set; }
}
