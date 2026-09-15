// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Everything about a report result except its rows - the figures that summarise it, when it ran, and
/// whether it is complete.
/// </summary>
/// <remarks>
/// Split out from <see cref="ReportResult{TRow}"/> so the parts of the UI that frame a report - the
/// summary strip, the truncation warning, the generated-at line - can be written once against this,
/// without a type parameter for rows they never touch.
/// </remarks>
public abstract class ReportResultBase
{
    /// <summary>
    /// Pre-formatted figures for the strip above the results - "12 organizations", "$14,203.00". Strings
    /// rather than numbers deliberately: a summary is displayed and never computed against, and keeping
    /// it as label/value pairs means every report can share one result shape instead of needing a
    /// summary type of its own.
    /// </summary>
    public List<ReportSummaryValueDto> Summary { get; set; } = [];

    /// <summary>When this ran. Shown on the page so a reader knows how stale the numbers are.</summary>
    public DateTimeOffset GeneratedOn { get; set; }

    /// <summary>
    /// True when the result hit <see cref="RowLimit"/> and rows were left off the end.
    /// </summary>
    /// <remarks>
    /// Reports load their whole result set, which is what makes client-side sorting and export
    /// trivially correct. This flag is the tripwire for that decision: a report that starts setting it
    /// has outgrown the approach and wants server-side paging - and gives up client-side export at the
    /// same time. It is surfaced in the UI rather than logged, because silently truncated numbers are
    /// worse than no numbers.
    /// </remarks>
    public bool Truncated { get; set; }

    /// <summary>The cap <see cref="Truncated"/> refers to.</summary>
    public int RowLimit { get; set; }
}

/// <summary>
/// What every report endpoint returns: the rows, the figures that summarise them, and when it was run.
/// </summary>
/// <remarks>
/// <para>
/// The shape exists so that a report is <em>an endpoint plus a row type</em> and nothing more. The Panel
/// is one consumer of that endpoint; the CSV export is a second; a scheduled or emailed report later is
/// a third. None of them needs the reporting component to reach into the database, which is what keeps
/// them from diverging.
/// </para>
/// <para>
/// <strong><see cref="ReportResultBase.Summary"/> is computed server-side, over the whole result.</strong>
/// Totalling in the browser instead would work right up until a report exceeded
/// <see cref="ReportResultBase.RowLimit"/>, at which point the total would quietly start describing only
/// the rows that happened to be loaded - a wrong number that looks exactly like a right one.
/// </para>
/// </remarks>
/// <typeparam name="TRow">The row type this report returns - one per report.</typeparam>
public class ReportResult<TRow> : ReportResultBase
{
    public List<TRow> Rows { get; set; } = [];
}

/// <summary>
/// One choice in a report's dropdown filter - a plan, a status, whatever the report narrows by.
/// </summary>
/// <remarks>
/// Values are strings because they end up in the page's query string either way (see the Panel's
/// <c>SelectParameter</c>), and a report that filters by plan id and one that filters by a status name
/// have no reason to need different endpoints to populate their dropdowns.
/// </remarks>
public class ReportFilterOptionDto
{
    /// <summary>What goes in the URL and comes back as the filter value.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>What the dropdown shows.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Optional grouping, for a dropdown long enough to want <c>optgroup</c>s.</summary>
    public string? Group { get; set; }
}

/// <summary>
/// One payment attempt - successful or failed - for the Payment Ledger report. The processor
/// reconciliation and failed-charge-tracking report in one: every row carries the provider's own id so
/// it can be cross-checked against Stripe's own dashboard/export, and a failed row carries why.
/// </summary>
public class PaymentLedgerRowDto
{
    public Guid PaymentId { get; set; }
    public Guid TenantId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    /// <summary>When this attempt happened - a receipt date for a success, when the decline was recorded
    /// for a failure.</summary>
    public DateTimeOffset ReceivedOn { get; set; }

    /// <summary>The payment provider's own id for the charge, when there is one - what an admin
    /// cross-checks against Stripe's own dashboard.</summary>
    public string? ProviderPaymentId { get; set; }

    /// <summary>Set only for a manually-entered payment - a cheque number, a bank reference.</summary>
    public string? Reference { get; set; }

    /// <summary>Stripe's machine-readable decline reason, set only for a <see cref="PaymentStatus.Failed"/> row.</summary>
    public string? FailureCode { get; set; }

    /// <summary>Stripe's human-readable decline reason, set only for a <see cref="PaymentStatus.Failed"/> row.</summary>
    public string? FailureMessage { get; set; }

    /// <summary>Invoice number(s) this payment settled, comma-joined - empty for a failed attempt (nothing
    /// was ever allocated) or a success still sitting unallocated.</summary>
    public string InvoiceNumbers { get; set; } = string.Empty;
}

/// <summary>
/// One group of Organizations sharing something they normally wouldn't - the same physical server, or
/// the same contact email - where at least one of them has redeemed a discount. A flag for a human to
/// look at, never an automatic conclusion: a legitimate business reorganizing looks identical to someone
/// re-registering to reuse a one-per-organization code, and only a person can tell the two apart. See
/// <c>ReportingService.GetDiscountAbuseSignalsAsync</c>'s own remarks.
/// </summary>
public class DiscountAbuseSignalRowDto
{
    /// <summary>"Shared server" or "Shared contact email".</summary>
    public string SignalType { get; set; } = string.Empty;

    /// <summary>What's actually shared - a "host:port" pair, or the email address itself.</summary>
    public string Detail { get; set; } = string.Empty;

    public List<DiscountAbuseTenantDto> Tenants { get; set; } = [];
}

/// <summary>One Organization inside a <see cref="DiscountAbuseSignalRowDto"/> group.</summary>
public class DiscountAbuseTenantDto
{
    public Guid TenantId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    /// <summary>Discount codes this Organization has redeemed - empty if none, included so the group
    /// reads as a whole rather than needing a second lookup.</summary>
    public List<string> RedeemedDiscountCodes { get; set; } = [];
}

/// <summary>
/// One jurisdiction currently blocking one or more tenants' invoices for lack of a Stripe tax
/// registration - see <c>RustArchon.Api.Data.BlockedInvoiceIssuance</c>.
/// </summary>
public class BlockedInvoiceJurisdictionRowDto
{
    /// <summary>ISO 3166-1 alpha-2 country code.</summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>State/province, when the jurisdiction has one.</summary>
    public string? State { get; set; }

    /// <summary>How many tenants currently have an invoice blocked in this jurisdiction.</summary>
    public int OrganizationCount { get; set; }

    /// <summary>The earliest <c>FirstBlockedOn</c> among them - how long this has been going on.</summary>
    public DateTimeOffset OldestBlockedOn { get; set; }
}

/// <summary>One figure in a report's summary strip.</summary>
public class ReportSummaryValueDto
{
    public string Label { get; set; } = string.Empty;

    /// <summary>The figure, already formatted for display - currency symbols, thousands separators and
    /// all. See <see cref="ReportResultBase.Summary"/> for why this isn't a number.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Optional smaller line under the value - a qualifier, a caveat, a breakdown.</summary>
    public string? Detail { get; set; }
}
