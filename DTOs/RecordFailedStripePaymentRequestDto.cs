// Copyright ©2026 Scott Blomfield

using System;
using System.ComponentModel.DataAnnotations;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Request body for <c>POST /internal/stripe/payments/failed</c> - the counterpart to
/// <see cref="RecordStripePaymentRequestDto"/> for a Stripe <c>payment_intent.payment_failed</c> event,
/// called from the same public webhook route once it has verified the event's signature.
/// </summary>
/// <remarks>
/// A separate DTO and endpoint from <see cref="RecordStripePaymentRequestDto"/>, not the same one with
/// optional fields - a success allocates money against an invoice and can reactivate a suspended tenant;
/// a failure does neither and only ever records that a decline happened. Keeping them distinct means a
/// caller can't accidentally record a decline through the success path (or vice versa) by leaving a field
/// blank.
/// </remarks>
public class RecordFailedStripePaymentRequestDto
{
    [Required]
    public Guid InvoiceId { get; set; }

    /// <summary>What the customer was attempting to pay - see <c>PaymentIntent.Amount</c>.</summary>
    public decimal Amount { get; set; }

    /// <summary>The PaymentIntent's own id.</summary>
    [Required]
    public string ProviderPaymentId { get; set; } = string.Empty;

    /// <summary>
    /// The webhook event's own id - what makes this call idempotent, since a single PaymentIntent can
    /// fail more than once (see <c>RustArchon.Api.Data.Payment.ProviderEventId</c>'s own remarks for why
    /// this can't be <see cref="ProviderPaymentId"/> instead).
    /// </summary>
    [Required]
    public string ProviderEventId { get; set; } = string.Empty;

    /// <summary>Stripe's machine-readable decline reason (<c>last_payment_error.code</c>), when there is one.</summary>
    public string? FailureCode { get; set; }

    /// <summary>Stripe's human-readable decline reason (<c>last_payment_error.message</c>), when there is one.</summary>
    public string? FailureMessage { get; set; }
}
