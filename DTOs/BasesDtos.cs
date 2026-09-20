// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>Someone authorized on a tool cupboard.</summary>
public class BaseAuthorizedPlayerDto
{
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>One tool cupboard: a base, as far as the plugin can tell.</summary>
public class BaseTcDto
{
    /// <summary>The plugin's id for the entity. Stable while the entity lives; not across a server restart.</summary>
    public int Id { get; set; }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    /// <summary>The SteamID64 of the player who placed it.</summary>
    public string OwnerId { get; set; } = string.Empty;

    public List<BaseAuthorizedPlayerDto> Authorized { get; set; } = new();
}

/// <summary>A server's tool cupboards as of the last time the plugin was read.</summary>
public class BasesDto
{
    /// <summary>False when the plugin's initial scan of the world had not finished, so cupboards may be missing.</summary>
    public bool Ready { get; set; }

    /// <summary>When the Worker read the plugin. Null when nothing has been read yet.</summary>
    public DateTimeOffset? CapturedAtUtc { get; set; }

    public List<BaseTcDto> Tcs { get; set; } = new();
}
