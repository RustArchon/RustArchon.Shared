// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>POST /internal/stripe/payments</c> - the shared-secret-authenticated endpoint
/// RustArchon.Panel's own public Stripe webhook route calls once it has verified a
/// <c>checkout.session.completed</c> event's signature and confirmed the session actually completed as
/// paid.
/// </summary>
/// <remarks>
/// Deliberately carries only what <c>IPaymentService.RecordPaymentAsync</c> needs, not the raw Stripe
/// event - RustArchon.Api has no Stripe SDK dependency of its own for the inbound side (see
/// <c>StripeCheckoutService</c>'s remarks: it only ever calls Stripe, never receives from it), and this
/// keeps that true. All the Stripe-specific parsing and signature verification happens once, in the
/// Panel, which is the only side of this codebase that's ever publicly reachable in the first place.
/// </remarks>
public class RecordStripePaymentRequestDto
{
    [Required]
    public Guid InvoiceId { get; set; }

    /// <summary>The amount Stripe actually confirms was collected - see <c>Session.AmountTotal</c>.</summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Stripe's own id for the actual charge (a PaymentIntent id, or the Checkout Session id as a
    /// fallback) - what makes this call idempotent against Stripe's at-least-once webhook delivery. See
    /// <c>IPaymentService.RecordPaymentAsync</c>'s own remarks.
    /// </summary>
    [Required]
    public string ProviderPaymentId { get; set; } = string.Empty;
}
