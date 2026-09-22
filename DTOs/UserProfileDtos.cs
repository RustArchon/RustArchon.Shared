// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// A person's own settings, as the application holds them. Kept in the main database, keyed by the identity user's id - not on the identity user,
/// which is left to be identity - so that anything in the Api can read them directly, and new settings are added here.
/// </summary>
public class UserProfileDto
{
    public Guid UserId { get; set; }

    /// <summary>The language (e.g. <c>en-US</c>) this person reads, or null if they have not chosen one (then the platform default applies).</summary>
    public string? PreferredCulture { get; set; }
}

/// <summary>Changing a person's settings. Every setting is sent each time (this replaces the profile's settings), so an omitted one clears it.</summary>
public class UpdateUserProfileDto
{
    /// <summary>A culture name such as <c>en-US</c> or <c>es</c>; null or empty clears it.</summary>
    [RegularExpression(@"^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,8}){0,3}$", ErrorMessage = "That is not a culture name such as en-US.")]
    [MaxLength(35)]
    public string? PreferredCulture { get; set; }
}

/// <summary>One person's language, sent when the Panel hands over what the identity database already held.</summary>
public class BackfillUserProfileItemDto
{
    public Guid UserId { get; set; }

    [RegularExpression(@"^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,8}){0,3}$")]
    [MaxLength(35)]
    public string? PreferredCulture { get; set; }
}

/// <summary>A batch of what the identity database held, to be recorded where it has none.</summary>
public class BackfillUserProfilesDto
{
    [MaxLength(1000)]
    public List<BackfillUserProfileItemDto> Items { get; set; } = [];
}
