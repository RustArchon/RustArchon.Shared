// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>An invoice as the billing admin screen shows it, with its lines and settlement history.</summary>
public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public string? Number { get; set; }
    public InvoiceStatus Status { get; set; }
    public string Currency { get; set; } = "USD";

    public DateTimeOffset? IssuedOn { get; set; }
    public DateTimeOffset? DueOn { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountCredited { get; set; }

    /// <summary>Total less paid less credited. Never negative.</summary>
    public decimal Outstanding { get; set; }

    /// <summary>Days past <see cref="DueOn"/>. Negative means still to run; zero when not applicable.</summary>
    public int DaysOverdue { get; set; }

    public DateTimeOffset? VoidedOn { get; set; }
    public DateTimeOffset? WrittenOffOn { get; set; }

    public List<InvoiceLineDto> Lines { get; set; } = [];

    /// <summary>Settlements against this invoice, including reversed ones.</summary>
    public List<InvoiceSettlementDto> Settlements { get; set; } = [];
}

public class InvoiceLineDto
{
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset ServiceStart { get; set; }
    public DateTimeOffset ServiceEnd { get; set; }
    public int Quantity { get; set; }
    public decimal UnitAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
}

/// <summary>
/// One thing that reduced what an invoice owes - a payment allocation or a credit note.
/// </summary>
/// <remarks>
/// The two are shown together because from the invoice's side they answer the same question: why is the
/// balance what it is. They stay separate rows underneath, because money arriving and value granted back
/// are entirely different events.
/// </remarks>
public class InvoiceSettlementDto
{
    public Guid Id { get; set; }

    /// <summary>Either <c>Payment</c> or <c>Credit note</c>.</summary>
    public string Kind { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public DateTimeOffset AppliedOn { get; set; }

    /// <summary>How the money arrived, for a payment. Null for a credit note.</summary>
    public PaymentMethod? Method { get; set; }

    /// <summary>A cheque number, bank reference, or the reason on a credit note.</summary>
    public string? Reference { get; set; }

    /// <summary>Set once this settlement has been reversed <em>in full</em> - still null while only
    /// part of it has (see <see cref="ReversedAmount"/>).</summary>
    public DateTimeOffset? ReversedOn { get; set; }

    /// <summary>How much of <see cref="Amount"/> has been given back so far, via one or more partial
    /// refunds - zero if none has. Always for a payment; a credit note is never itself reversed.</summary>
    public decimal ReversedAmount { get; set; }
}

/// <summary>
/// A payment being recorded by hand - a bank transfer reconciled off a statement, a cheque.
/// </summary>
/// <remarks>
/// <see cref="InvoiceId"/> names where the money should go first. Anything left over after settling that
/// invoice is applied to the same Organization's other open invoices, oldest due first, and whatever
/// still remains stays unallocated against the account - which is a real state, not an error, and one
/// the invoice screen shows rather than hides.
/// </remarks>
public class RecordPaymentRequestDto
{
    public Guid InvoiceId { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.Manual;

    /// <summary>When the money actually arrived, which need not be when this is entered.</summary>
    public DateTimeOffset? ReceivedOn { get; set; }

    public string? Reference { get; set; }
}

/// <summary>Value granted back against an invoice without money moving.</summary>
public class IssueCreditNoteRequestDto
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }

    /// <summary>Required - an unexplained credit is indistinguishable from a mistake.</summary>
    public string Reason { get; set; } = string.Empty;
}

/// <summary>Closing an invoice without payment - see <see cref="InvoiceStatus"/> for which to use.</summary>
public class CloseInvoiceRequestDto
{
    public Guid InvoiceId { get; set; }

    /// <summary>Recorded as the credit note's reason when voiding leaves a trail worth keeping.</summary>
    public string? Reason { get; set; }
}
