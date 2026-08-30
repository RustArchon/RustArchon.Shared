// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Response for <c>GET /api/invitations/status</c> - lets the Register page decide whether to show
/// and require the invitation-code field before an account exists at all (so there's no user/tenant
/// context yet to authenticate a normal API call with).
/// </summary>
public class InvitationStatusDto
{
    /// <summary>
    /// Whether registration currently requires a valid invitation code. Mirrors
    /// <c>RustArchon.Api.Infrastructure.InvitationCodeOptions.Enabled</c> - the sign-up kill switch.
    /// </summary>
    public bool Enabled { get; set; }
}

/// <summary>
/// Request body for <c>POST /api/invitations/redeem</c>.
/// </summary>
public class RedeemInvitationCodeRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Result of a redemption attempt. Always returned with HTTP 200 - an invalid/used code is an
/// expected outcome of this endpoint, not a server error, so the caller can check
/// <see cref="Success"/> without catching an HTTP exception for the common failure case.
/// </summary>
public class RedeemInvitationCodeResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}
