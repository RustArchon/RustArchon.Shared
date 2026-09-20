// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>One recorded position of one player.</summary>
public class PositionSampleDto
{
    /// <summary>The plugin's sequence number (per run of the plugin); only orders samples that share a run.</summary>
    public long Sequence { get; set; }

    public DateTimeOffset OccurredAtUtc { get; set; }

    /// <summary>The player's SteamID64.</summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>The player's name, when the plugin included it (the first sample, heartbeats and the last one); otherwise empty.</summary>
    public string PlayerName { get; set; } = string.Empty;

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    /// <summary>Which way the player was facing, whole degrees 0 to 359.</summary>
    public int Yaw { get; set; }

    /// <summary>"on" for the first sample after the player appeared, "off" for the last as they left, otherwise empty.</summary>
    public string Marker { get; set; } = string.Empty;
}

/// <summary>Recorded positions for one server, newest first.</summary>
public class PositionsDto
{
    public List<PositionSampleDto> Samples { get; set; } = new();

    /// <summary>True when more matched than were returned.</summary>
    public bool HasMore { get; set; }
}
