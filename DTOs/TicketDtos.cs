// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>Mirrors <c>RustArchon.Api.Data.TicketMessageAuthorType</c> - see <see cref="TicketStatusDto"/>'s
/// remarks.</summary>
public enum TicketMessageAuthorType
{
    Customer,
    Staff
}

/// <summary>
/// One row in a ticket list - the staff console's and a tenant's "My Tickets" table. See
/// <see cref="TicketDetailDto"/> for the full record a click opens.
/// </summary>
public class TicketSummaryDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public TicketStatusDto Status { get; set; } = new();
    public Guid QueueId { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public string SubmitterName { get; set; } = string.Empty;
    public string SubmitterEmail { get; set; } = string.Empty;
    public Guid? AssignedToUserId { get; set; }
    public DateTimeOffset SubmittedOn { get; set; }
    public DateTimeOffset? ResolvedOn { get; set; }
    public DateTimeOffset? ClosedOn { get; set; }

    /// <summary>
    /// A staff-set abuse guard on this one ticket: while it's closed, a customer/guest reply is refused
    /// instead of reopening it - see <c>RustArchon.Api.Data.Ticket.PreventReopening</c>.
    /// </summary>
    public bool PreventReopening { get; set; }
}

/// <summary>
/// The full record behind one ticket - what a click on a <see cref="TicketSummaryDto"/> row opens.
/// <see cref="Notes"/> is populated only for the staff console; a tenant or guest thread view never
/// receives it - see <c>TicketsController</c>.
/// </summary>
public class TicketDetailDto : TicketSummaryDto
{
    public IReadOnlyList<TicketMessageDto> Messages { get; set; } = Array.Empty<TicketMessageDto>();

    public IReadOnlyList<TicketNoteDto>? Notes { get; set; }
}

public class TicketMessageDto
{
    public Guid Id { get; set; }
    public TicketMessageAuthorType AuthorType { get; set; }
    public Guid? AuthorUserId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
}

public class TicketNoteDto
{
    public Guid Id { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
}

/// <summary>A tenant user's request to open a new ticket - the "New Ticket" form.</summary>
public class CreateTicketRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    public Guid QueueId { get; set; }

    [Required]
    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;
}

/// <summary>A reply added to a ticket's customer-visible thread - by either party.</summary>
public class SaveTicketMessageRequestDto
{
    [Required]
    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;
}

/// <summary>
/// A newly-registered user's request to claim a guest ticket as their own, submitted right after
/// sign-up when they arrived via a guest ticket's "Sign up" link. The token is the only thing the
/// client asserts - the Api resolves the ticket from it and only relinks if the ticket's own
/// <c>SubmitterEmail</c> matches the caller's own verified (signed-in) email, never the reverse.
/// </summary>
public class ClaimGuestTicketRequestDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}

/// <summary>An internal, staff-only note added to a ticket.</summary>
public class SaveTicketNoteRequestDto
{
    [Required]
    [MaxLength(4000)]
    public string Content { get; set; } = string.Empty;
}

/// <summary>A staff-only change to a ticket's status, queue, assignee, or prevent-reopening flag.</summary>
public class UpdateTicketRequestDto
{
    public Guid StatusId { get; set; }

    public Guid QueueId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public bool PreventReopening { get; set; }
}
