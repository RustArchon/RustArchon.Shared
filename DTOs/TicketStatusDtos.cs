// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>A state a <see cref="TicketSummaryDto"/> can be in - "Open," "Resolved," "Cancelled."
/// Admin-managed master data - see <c>RustArchon.Api.Data.TicketStatus</c>.</summary>
public class TicketStatusDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    /// <summary>Whether a ticket in this status is done - hidden from the staff console's default
    /// queue view.</summary>
    public bool IsClosed { get; set; }

    /// <summary>Whether ticket-lifecycle code depends on this exact status by <see cref="Slug"/> - a
    /// protected status can be renamed or have <see cref="IsClosed"/> changed, but never deleted or
    /// deactivated.</summary>
    public bool IsProtected { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}

/// <summary>A new custom status for the admin ticket-status management page to create.</summary>
public class CreateTicketStatusRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsClosed { get; set; }

    public int DisplayOrder { get; set; }
}

/// <summary>A change to an existing status's name, closed flag, display order, or active flag. Never
/// touches <see cref="TicketStatusDto.Slug"/> or <see cref="TicketStatusDto.IsProtected"/> - both are
/// fixed at creation.</summary>
public class UpdateTicketStatusRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsClosed { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
