// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>POST /internal/email/templated</c> - the shared-secret-authenticated endpoint
/// the Blazor web app's <c>QueuedEmailSender</c> calls to have an Identity email (confirmation, password
/// reset) queued through an admin-editable <c>EmailTemplate</c> instead of raw HTML built in the Panel.
/// See <c>RustArchon.Api.Infrastructure.EmailTemplateRegistry.Codes</c> for valid <see cref="TemplateCode"/>
/// values.
/// </summary>
public class SendTemplatedEmailRequestDto
{
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    [Required]
    public string TemplateCode { get; set; } = string.Empty;

    /// <summary>Values for the template's <c>{{Token}}</c> placeholders, keyed by
    /// <c>EmailPlaceholder.Name</c>.</summary>
    public Dictionary<string, string> Tokens { get; set; } = [];

    /// <summary>The member this is to - see <c>Communication.UserId</c>'s remarks. Always set for this
    /// endpoint's callers (every Identity email is about a real account), unlike the plain
    /// <see cref="SendEmailRequestDto"/> where it's optional.</summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The recipient's preferred culture (e.g. <c>"en-US"</c>), from
    /// <c>RustArchon.Panel.Data.ApplicationUser.PreferredCulture</c> - passed through unvalidated to
    /// <c>ICommunicationPublisher.QueueTemplatedAsync</c>, whose own fallback chain handles a null,
    /// empty, or unrecognized value the same way as a recognized one with no translation saved yet.
    /// </summary>
    public string? Culture { get; set; }
}
