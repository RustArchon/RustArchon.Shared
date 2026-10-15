// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>What can be shown about the diagnostic log token. Never the secret.</summary>
public class DiagnosticLogTokenStatusDto
{
    /// <summary>False when no token has ever been created: the diagnostic log endpoint is off.</summary>
    public bool Exists { get; set; }

    /// <summary>The token's short id (the part before the dot), safe to show and to quote in the logs.</summary>
    public string TokenId { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAtUtc { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>When it was revoked, or null while it is still the live token.</summary>
    public DateTimeOffset? RevokedAtUtc { get; set; }
}

/// <summary>A newly created diagnostic log token. <see cref="Token"/> is shown once, here, and can never be retrieved again.</summary>
public class DiagnosticLogTokenCreatedDto
{
    public string Token { get; set; } = string.Empty;

    public string TokenId { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }
}
