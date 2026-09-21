// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One reason a plugin file on a server did not load, as Carbon lists it under "failed plugins": the file, the place in it, and what the compiler said.
/// A file that failed for several reasons has one of these for each. Text that came from the game server: shown as text, never as markup.
/// </summary>
public class ServerPluginFailureDto
{
    /// <summary>The plugin's file name, for example <c>BotReSpawn.cs</c>.</summary>
    public string FileName { get; set; } = string.Empty;

    public int Line { get; set; }
    public int Column { get; set; }

    /// <summary>The compiler's message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>When the plugin-list poll that last saw this ran.</summary>
    public DateTimeOffset CapturedAtUtc { get; set; }
}
