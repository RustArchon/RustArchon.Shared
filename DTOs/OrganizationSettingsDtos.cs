// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>An Organization's own editable identity - its name, the address its notices go to, and its
/// billing address.</summary>
public class OrganizationSettingsDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    // ---- Billing address - see TenantBillingAddress's own remarks. All optional; Country is the one
    // field anything downstream (tax calculation) actually depends on, and even that's only enforced
    // once a caller tries to use it for something, not here.
    public string? BillingLine1 { get; set; }
    public string? BillingLine2 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingPostalCode { get; set; }

    /// <summary>ISO 3166-1 alpha-2 country code (e.g. "US"), or null if no billing address is on file
    /// yet.</summary>
    public string? BillingCountry { get; set; }
}

/// <summary>A request to change an Organization's own name, contact address, or billing address.</summary>
public class UpdateOrganizationSettingsRequestDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Cleared, not refused, when left blank - not every Organization has to set one.</summary>
    [OptionalEmailAddress]
    [StringLength(255)]
    public string? ContactEmail { get; set; }

    // ---- Billing address - see OrganizationSettingsDto's own remarks. Saving with BillingCountry
    // blank clears any billing address on file entirely, the same "blank clears it" rule ContactEmail
    // already follows - see IOrganizationSettingsService.UpdateAsync's remarks.
    [StringLength(200)]
    public string? BillingLine1 { get; set; }

    [StringLength(200)]
    public string? BillingLine2 { get; set; }

    [StringLength(100)]
    public string? BillingCity { get; set; }

    [StringLength(100)]
    public string? BillingState { get; set; }

    [StringLength(20)]
    public string? BillingPostalCode { get; set; }

    [StringLength(2, MinimumLength = 2)]
    public string? BillingCountry { get; set; }
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
