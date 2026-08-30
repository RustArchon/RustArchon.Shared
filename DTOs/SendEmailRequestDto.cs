// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>POST /internal/email</c> - the shared-secret-authenticated endpoint the Blazor
/// web app's <c>QueuedEmailSender</c> calls to have an email queued for delivery. See
/// <c>RustArchon.Messaging.Contracts.EmailRequested</c>, the message this gets turned into.
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
}
