// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>POST /internal/stripe/disputes</c> - called from RustArchon.Panel's own public
/// Stripe webhook route on a verified <c>charge.dispute.created</c> event.
/// </summary>
public class RecordStripeDisputeRequestDto
{
    /// <summary>The disputed charge's PaymentIntent id - how this is matched back to a recorded payment.</summary>
    [Required]
    public string ProviderPaymentId { get; set; } = string.Empty;

    /// <summary>Stripe's own id for the dispute (a <c>dp_...</c> id) - what makes this call idempotent
    /// against Stripe's at-least-once webhook delivery.</summary>
    [Required]
    public string DisputeId { get; set; } = string.Empty;

    /// <summary>Stripe's own reason code (e.g. <c>fraudulent</c>, <c>product_not_received</c>).</summary>
    public string? Reason { get; set; }

    /// <summary>Stripe's own deadline for submitting evidence (<c>evidence_details.due_by</c>).</summary>
    public DateTimeOffset? DueBy { get; set; }
}
