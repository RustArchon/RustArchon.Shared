// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>A named place on the map, in world coordinates (metres, the world's centre is 0,0; z runs north).</summary>
public class MapMonumentDto
{
    public string Name { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}

/// <summary>What the Panel knows about a server's current world map.</summary>
public class MapDto
{
    /// <summary>True once the picture has reached the Panel. False while it has not (or the server has no plugin).</summary>
    public bool Available { get; set; }

    /// <summary>The world's size in metres (square) and seed; the picture covers exactly this area.</summary>
    public int WorldSize { get; set; }
    public long WorldSeed { get; set; }

    public DateTimeOffset? UploadedAtUtc { get; set; }

    /// <summary>The picture's size in bytes, and a value that changes when the picture does (the HTTP entity tag, without quotes).</summary>
    public long ImageBytes { get; set; }
    public string? ImageEtag { get; set; }

    public List<MapMonumentDto> Monuments { get; set; } = new();
}
