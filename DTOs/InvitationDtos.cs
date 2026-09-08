// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>An outstanding offer for somebody to join an Organization.</summary>
/// <remarks>
/// Carries no token. The secret goes in exactly one place - the emailed link - and putting it on a
/// listing DTO would spread it to every screen and log that renders one.
/// </remarks>
public class OrganizationInvitationDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    /// <summary>The role they will hold on joining, or <c>null</c> for none.</summary>
    public Guid? RoleId { get; set; }

    /// <summary>That role's name, so the listing needs no second call to render.</summary>
    public string? RoleName { get; set; }

    public DateTimeOffset InvitedOn { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }
}

/// <summary>A request to invite one person into the caller's own Organization.</summary>
public class InviteMemberRequestDto
{
    public string Email { get; set; } = string.Empty;

    /// <summary>A role to grant them on joining. Must be one the inviter could grant directly.</summary>
    public Guid? RoleId { get; set; }
}

/// <summary>What an invitation link shows before anybody accepts it.</summary>
/// <remarks>
/// Deliberately thin: the Organization's name, who it was sent to, and whether it still works. Enough
/// for somebody to recognise the invitation as theirs, and nothing about the Organization's members,
/// servers or plan - this is readable by whoever holds the link, who is not a member yet.
/// </remarks>
public class InvitationPreviewDto
{
    public string OrganizationName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTimeOffset ExpiresOn { get; set; }

    /// <summary>False when the link is spent, withdrawn or expired.</summary>
    public bool IsPending { get; set; }
}

/// <summary>The result of accepting an invitation.</summary>
public class AcceptInvitationResultDto
{
    /// <summary>Whether the caller now belongs to the Organization.</summary>
    public bool Joined { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public Guid? TenantId { get; set; }

    /// <summary>
    /// A sentence for the screen, written for the specific reason this succeeded or failed.
    /// </summary>
    /// <remarks>
    /// Composed on the server so every client says the same thing, and so the difference between
    /// "expired", "already used" and "sent to a different address" survives the trip - collapsing
    /// them into one "invalid link" is what makes a person give up rather than sign in with their
    /// other address.
    /// </remarks>
    public string Message { get; set; } = string.Empty;
}
