// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;
using JumpStart.Api.DTOs;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// DTO for reading an invitation code, used by the platform-admin management page.
/// </summary>
public class InvitationCodeDto : AuditableEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? BoundEmail { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? RedeemedAtUtc { get; set; }
    public string? RedeemedByEmail { get; set; }
}

/// <summary>
/// DTO for minting a new invitation code. The code string itself is generated server-side - see
/// <c>RustArchon.Api.Infrastructure.Security.InvitationCodeGenerator</c> - and never accepted here.
/// </summary>
public class CreateInvitationCodeDto : ICreateDto
{
    [MaxLength(200)]
    public string? Note { get; set; }

    /// <summary>
    /// Restricts the code to one email address. Leave <c>null</c> to let anyone redeem it first.
    /// </summary>
    [EmailAddress]
    [MaxLength(256)]
    public string? BoundEmail { get; set; }
}

/// <summary>
/// DTO for editing an existing invitation code. Only <see cref="Note"/> and <see cref="IsActive"/>
/// are editable - <c>Code</c>/<c>BoundEmail</c> are fixed at creation, and redemption state is only
/// ever written by the atomic redeem operation, never through this DTO.
/// </summary>
public class UpdateInvitationCodeDto : IUpdateDto
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public string? Note { get; set; }

    /// <summary>
    /// Set to <c>false</c> to revoke this specific code before it's used.
    /// </summary>
    public bool IsActive { get; set; }
}
