// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// What the marketing site's contact form needs before it can render itself - which captcha widget (if
/// any) to load, and the queue picker's options. Served anonymously by
/// <c>PublicTicketingConfigController</c>, the same <c>[AllowAnonymous]</c>-on-its-own-controller
/// pattern as <c>PublicBrandingController</c>.
/// </summary>
public class PublicTicketingConfigDto
{
    /// <summary>One of <c>RustArchon.Api.Infrastructure.PlatformSettingsRegistry.CaptchaProviders</c> -
    /// "None" means the form renders no captcha widget at all.</summary>
    public string CaptchaProvider { get; set; } = string.Empty;

    /// <summary>The public key the captcha widget renders with. Empty when <see cref="CaptchaProvider"/>
    /// is "None".</summary>
    public string? CaptchaSiteKey { get; set; }

    public List<QueueDto> Queues { get; set; } = [];
}

/// <summary>An anonymous submission from the marketing site's contact form.</summary>
public class SubmitPublicTicketRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    public Guid QueueId { get; set; }

    [Required]
    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    /// <summary>The captcha widget's token. Required unless the platform's
    /// <see cref="PublicTicketingConfigDto.CaptchaProvider"/> is "None".</summary>
    public string? CaptchaToken { get; set; }

    /// <summary>
    /// A field no real visitor ever fills in - hidden from sighted users via CSS, still present in the
    /// DOM for a scripted submission that fills in every field it finds. Non-empty here means the
    /// submission is silently dropped (see <c>TicketSubmissionController.Submit</c>) - never surfaced
    /// to the sender, so a bot has no signal to adapt against.
    /// </summary>
    public string? Website { get; set; }
}

/// <summary>What a successful anonymous submission gets back - just enough to send the submitter to
/// their new ticket's guest-access page.</summary>
public record SubmitPublicTicketResponseDto(Guid TicketId, string GuestAccessToken);
