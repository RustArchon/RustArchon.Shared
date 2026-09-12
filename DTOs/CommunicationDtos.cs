// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Mirrors <c>RustArchon.Api.Data.CommunicationStatus</c> - duplicated here rather than shared via a
/// project reference, the same way DTOs never reference API entity types directly.
/// </summary>
public enum CommunicationStatus
{
    Queued,
    Sent,
    Bounced,
    Viewed,
    Cancelled
}

/// <summary>
/// One row in a communications list - the Organization/User admin screens' "Queue Date, Subject,
/// Status" table. See <c>CommunicationDetailDto</c> for the full record a click opens.
/// </summary>
public class CommunicationSummaryDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public DateTimeOffset QueuedOn { get; set; }
    public CommunicationStatus Status { get; set; }
}

/// <summary>
/// The full record behind one communication - what a click on a <see cref="CommunicationSummaryDto"/>
/// row opens. Carries the actual body, since the whole point is a permanent copy of what was sent.
/// </summary>
public class CommunicationDetailDto : CommunicationSummaryDto
{
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string HtmlBody { get; set; } = string.Empty;
    public DateTimeOffset? SentOn { get; set; }
    public DateTimeOffset? BouncedOn { get; set; }
    public DateTimeOffset? ViewedOn { get; set; }
    public DateTimeOffset? CancelledOn { get; set; }
    public string? FailureReason { get; set; }

    /// <summary>Whether <c>CommunicationsController.Cancel</c> would currently accept a cancel for
    /// this row - true only while <see cref="CommunicationSummaryDto.Status"/> is still Queued.</summary>
    public bool CanCancel { get; set; }
}
