// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>PlatformSettingsController.TestEmail</c> - where to send the test message.
/// Subject/body aren't caller-supplied; every test email says the same fixed, harmless thing, so
/// there's nothing here for a site admin to accidentally send something real-looking to a real address.
/// </summary>
public class SendTestEmailRequestDto
{
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;
}
