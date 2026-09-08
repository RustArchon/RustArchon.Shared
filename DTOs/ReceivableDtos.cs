// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// How overdue a debt is, in the buckets collections work is organised around.
/// </summary>
/// <remarks>
/// Derived from an invoice's due date at the moment a report runs, never stored - an invoice does not
/// change when it ages, and a stored bucket would be wrong every day after the one it was written on.
/// The boundaries are the conventional 30-day ones so the aging summary matches what any accountant
/// expects to see.
/// </remarks>
public enum AgingBucket
{
    /// <summary>Not yet due. Owed, but nobody is late.</summary>
    Current = 0,

    Days1To30 = 1,
    Days31To60 = 2,
    Days61To90 = 3,

    /// <summary>Over ninety days late - the point at which a debt is usually considered doubtful.</summary>
    Over90 = 4
}

/// <summary>
/// One open invoice and what is still owed on it.
/// </summary>
/// <remarks>
/// The invoice register, not the collections list - one row per document rather than per Organization,
/// because the question after "who owes us" is always "for what". <see cref="DelinquentAccountRowDto"/>
/// is the same data grouped the other way.
/// </remarks>
public class ReceivableRowDto
{
    public Guid TenantId { get; set; }
    public Guid InvoiceId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";

    public DateTimeOffset? IssuedOn { get; set; }
    public DateTimeOffset? DueOn { get; set; }

    /// <summary>
    /// Whole days past the due date. Zero or negative means not yet due - a negative value is days
    /// still to run, which is worth seeing rather than flattening to zero.
    /// </summary>
    public int DaysOverdue { get; set; }

    public AgingBucket Bucket { get; set; }

    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountCredited { get; set; }

    /// <summary>What is still owed: total less paid less credited.</summary>
    public decimal Outstanding { get; set; }

    /// <summary>True when something has been paid but not all of it.</summary>
    public bool IsPartiallyPaid { get; set; }

    /// <summary>The plan the Organization is on now - context for whether chasing is worth it.</summary>
    public string PlanName { get; set; } = string.Empty;
    public string PlanColorCode { get; set; } = string.Empty;
}

/// <summary>
/// One Organization with money outstanding, aged - the list somebody actually works down.
/// </summary>
/// <remarks>
/// Aggregated from <see cref="ReceivableRowDto"/>. Separate rather than left to the reader to total up,
/// because the collections decision is made per customer and not per document: whether to chase, and how
/// hard, depends on the whole relationship - how much, how old, and whether they are still a paying
/// subscriber.
/// </remarks>
public class DelinquentAccountRowDto
{
    public Guid TenantId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public string PlanName { get; set; } = string.Empty;
    public string PlanColorCode { get; set; } = string.Empty;

    /// <summary>How many open invoices this Organization has.</summary>
    public int OpenInvoices { get; set; }

    /// <summary>Everything owed, due or not.</summary>
    public decimal TotalOutstanding { get; set; }

    /// <summary>The part of <see cref="TotalOutstanding"/> that is actually past its due date.</summary>
    public decimal OverdueAmount { get; set; }

    /// <summary>Days since the oldest unpaid invoice fell due. Zero when nothing is overdue yet.</summary>
    public int OldestOverdueDays { get; set; }

    /// <summary>The bucket <see cref="OldestOverdueDays"/> falls in - how the list is prioritised.</summary>
    public AgingBucket WorstBucket { get; set; }

    /// <summary>Aged breakdown, so a row shows its own shape without opening the invoice list.</summary>
    public decimal CurrentAmount { get; set; }
    public decimal Days1To30 { get; set; }
    public decimal Days31To60 { get; set; }
    public decimal Days61To90 { get; set; }
    public decimal Over90 { get; set; }

    /// <summary>
    /// What this Organization is worth per month if it keeps paying - the other half of the decision
    /// about how hard to chase.
    /// </summary>
    public decimal MonthlyValue { get; set; }

    public bool IsActive { get; set; }
}
