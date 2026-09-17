// Copyright ©2026 Scott Blomfield

using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>POST /internal/stripe/disputes/closed</c> - called from RustArchon.Panel's own
/// public Stripe webhook route on a verified <c>charge.dispute.closed</c> event.
/// </summary>
public class RecordStripeDisputeClosedRequestDto
{
    /// <summary>Stripe's own id for the dispute - how this is matched back to the payment
    /// <c>charge.dispute.created</c> already tagged with it.</summary>
    [Required]
    public string DisputeId { get; set; } = string.Empty;

    /// <summary>Stripe's own final status (<c>won</c>, <c>lost</c>, or <c>warning_closed</c>), stored as-is.</summary>
    [Required]
    public string Status { get; set; } = string.Empty;
}
