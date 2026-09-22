// Copyright ©2026 Scott Blomfield

namespace RustArchon.Shared.DTOs;

/// <summary>
/// What came of asking a server to be polled right now (<c>POST api/rustservers/{id}/plugins/poll</c>), instead of waiting for the Worker's own
/// few-minute schedule. <see cref="Outcome"/> is one of:
/// <list type="bullet">
/// <item><c>polled</c> - the server answered; whatever the caller was about to read has just been refreshed.</item>
/// <item><c>not_connected</c> - a Worker owns this server's connection, but the connection itself is not up right now.</item>
/// <item><c>no_worker</c> - no Worker instance currently owns this server's connection (none is running, or claiming it has not finished).</item>
/// <item><c>rate_limited</c> - asked too recently; nothing was sent, and whatever is already stored is simply what is read.</item>
/// </list>
/// None of these is an error a caller needs to show as one: reading what is already stored right afterward is always safe, and in the common case
/// (<c>polled</c>) it is now current.
/// </summary>
public class ServerPollResultDto
{
    public string Outcome { get; set; } = string.Empty;
}
