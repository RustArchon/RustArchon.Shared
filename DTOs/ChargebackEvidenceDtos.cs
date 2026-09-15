// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Everything RustArchon can assemble in defense of a chargeback against one payment - the "chargeback
/// packet" a site admin reviews, prints/saves as a PDF from the browser, or submits directly to Stripe.
/// </summary>
/// <remarks>
/// Deliberately built from data this codebase already has (invoice/service-period detail, server
/// connection activity, communication delivery and open-tracking, account tenure) rather than anything
/// new like IP or page-visit analytics - see <c>ChargebackEvidenceService</c>'s own remarks.
/// </remarks>
public class ChargebackEvidenceDto
{
    public Guid PaymentId { get; set; }
    public string? DisputeId { get; set; }
    public string? DisputeReason { get; set; }
    public DateTimeOffset? DisputeDueBy { get; set; }
    public DateTimeOffset? DisputeEvidenceSubmittedOn { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? ProviderPaymentId { get; set; }
    public DateTimeOffset ReceivedOn { get; set; }

    public Guid TenantId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? BillingAddress { get; set; }

    /// <summary>When this Organization signed up - proof this isn't a first-and-only interaction.</summary>
    public DateTimeOffset CustomerSinceOn { get; set; }

    /// <summary>How many payments this Organization successfully made before this one.</summary>
    public int PriorSuccessfulPaymentCount { get; set; }

    public List<ChargebackEvidenceInvoiceDto> Invoices { get; set; } = [];
    public List<ChargebackEvidenceServerDto> Servers { get; set; } = [];
    public List<ChargebackEvidenceCommunicationDto> Communications { get; set; } = [];

    /// <summary>
    /// A pre-assembled narrative covering everything above - ready to paste into Stripe's
    /// <c>uncategorized_text</c> evidence field, and exactly what "Submit to Stripe" sends there
    /// automatically.
    /// </summary>
    public string SummaryText { get; set; } = string.Empty;
}

/// <summary>One invoice this payment settled, with its service period - the "what was being paid for,
/// and when" evidence.</summary>
public class ChargebackEvidenceInvoiceDto
{
    public string Number { get; set; } = string.Empty;
    public DateTimeOffset? IssuedOn { get; set; }
    public DateTimeOffset? DueOn { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset? ServiceStart { get; set; }
    public DateTimeOffset? ServiceEnd { get; set; }
    public List<string> LineDescriptions { get; set; } = [];
}

/// <summary>One of the Organization's servers, and how much RCON activity it saw during the disputed
/// service period - the "the service was actually running and administered" evidence.</summary>
public class ChargebackEvidenceServerDto
{
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public int ConnectionEventCount { get; set; }
    public DateTimeOffset? FirstActivityInPeriod { get; set; }
    public DateTimeOffset? LastActivityInPeriod { get; set; }
}

/// <summary>One email sent to the Organization around this payment - the "the customer was kept informed
/// and engaged with it" evidence.</summary>
public class ChargebackEvidenceCommunicationDto
{
    public string Subject { get; set; } = string.Empty;
    public DateTimeOffset? SentOn { get; set; }
    public DateTimeOffset? ViewedOn { get; set; }
}
