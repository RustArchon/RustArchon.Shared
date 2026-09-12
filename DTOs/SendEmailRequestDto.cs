// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>POST /internal/email</c> - the shared-secret-authenticated endpoint the Blazor
/// web app's <c>QueuedEmailSender</c> calls to have an email queued for delivery. See
/// <c>RustArchon.Messaging.Contracts.EmailRequested</c>, the message this gets turned into, and
/// <c>Communication</c>, the permanent record it also becomes.
/// </summary>
public class SendEmailRequestDto
{
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>The member this is to, when the caller knows - see <c>Communication.UserId</c>'s remarks.</summary>
    public Guid? UserId { get; set; }

    /// <summary>Set only for an organization-level communication - see <c>Communication.TenantId</c>'s remarks.</summary>
    public Guid? TenantId { get; set; }
}
