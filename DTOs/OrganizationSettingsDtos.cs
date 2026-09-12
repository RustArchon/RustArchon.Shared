// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>An Organization's own editable identity - its name and the address its notices go to.</summary>
public class OrganizationSettingsDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
}

/// <summary>A request to change an Organization's own name or contact address.</summary>
public class UpdateOrganizationSettingsRequestDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Cleared, not refused, when left blank - not every Organization has to set one.</summary>
    [OptionalEmailAddress]
    [StringLength(255)]
    public string? ContactEmail { get; set; }
}

/// <summary>
/// <see cref="EmailAddressAttribute"/>, but blank passes instead of failing.
/// </summary>
/// <remarks>
/// The built-in attribute only exempts <c>null</c> - an empty string fails its own regex check. That
/// is exactly the value a cleared Blazor <c>&lt;input&gt;</c> binds into a <c>string?</c> property
/// (HTML has no way to produce a literal <c>null</c>), so pairing <c>[EmailAddress]</c> directly with
/// an optional field a user is meant to be able to blank out - like <see cref="OrganizationSettingsDto.ContactEmail"/> -
/// makes clearing it impossible: the form reports "not a valid e-mail address" on an empty box and
/// refuses to submit.
/// </remarks>
public sealed class OptionalEmailAddressAttribute : ValidationAttribute
{
    private static readonly EmailAddressAttribute Inner = new();

    public override bool IsValid(object? value) =>
        value is not string text || string.IsNullOrWhiteSpace(text) || Inner.IsValid(value);
}
