// Copyright ©2026 Scott Blomfield

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Where an invoice stands.
/// </summary>
/// <remarks>
/// <para>
/// One progression and three terminal dispositions, not four peers. <see cref="Draft"/> to
/// <see cref="Open"/> is the progression - a draft is still accumulating lines and has no number;
/// finalising gives it both a number and a due date. <see cref="Paid"/>, <see cref="Void"/> and
/// <see cref="Uncollectible"/> are where it ends up.
/// </para>
/// <para>
/// <strong>Past due and partially paid are deliberately absent.</strong> Both are derived - an invoice
/// is past due when it is <see cref="Open"/> and its due date has passed, and partially paid when it is
/// <see cref="Open"/> with some but not all of its total allocated. Storing them would create a second
/// place for the truth to live, and the first thing that goes wrong with a flat status enum is illegal
/// states like voided-and-paid.
/// </para>
/// </remarks>
public enum InvoiceStatus
{
    /// <summary>Still accumulating lines. No number, no due date, not owed.</summary>
    Draft = 0,

    /// <summary>Issued and owed. Has a number and a due date.</summary>
    Open = 1,

    /// <summary>Settled in full by payments and credits.</summary>
    Paid = 2,

    /// <summary>
    /// Issued in error. Removed from receivables entirely - as though it had never been owed.
    /// </summary>
    /// <remarks>
    /// Not the same write-off as <see cref="Uncollectible"/>, and the difference is the one an auditor
    /// cares about: this says the debt never should have existed, that one says it was real and you gave
    /// up on it. A single "cancelled" state loses the distinction, and with it the bad-debt expense.
    /// The number is kept and stays in the sequence - see <c>InvoiceNumberSequence</c>.
    /// </remarks>
    Void = 3,

    /// <summary>
    /// Written off as bad debt. The debt was real and stays in the books as an expense - see
    /// <see cref="Void"/> for why the two are separate.
    /// </summary>
    Uncollectible = 4
}

/// <summary>
/// Where a payment stands.
/// </summary>
/// <remarks>
/// <c>Pending → Succeeded</c> or <c>Pending → Failed</c>; a succeeded payment can later become
/// <see cref="Refunded"/> or <see cref="Disputed"/>. Both of those reverse the payment's allocations
/// rather than editing the invoice, so an invoice reopens by arithmetic instead of by a status being
/// edited back - which is what keeps the invoice immutable once finalised.
/// </remarks>
public enum PaymentStatus
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2,

    /// <summary>Returned to the payer. Allocations are reversed rather than the invoice being edited.</summary>
    Refunded = 3,

    /// <summary>Charged back by the payer's bank. Same reversal as <see cref="Refunded"/>.</summary>
    Disputed = 4
}

/// <summary>How money arrived.</summary>
/// <remarks>
/// <see cref="Manual"/> is first because it is the one that works before any provider integration
/// exists - recording a payment by hand is what exercises allocation, partial payment and write-off
/// with nothing to integrate against.
/// </remarks>
public enum PaymentMethod
{
    /// <summary>Entered by an administrator - a bank transfer reconciled by hand, a cheque, a credit.</summary>
    Manual = 0,

    Card = 1,
    BankTransfer = 2,
    Other = 99
}
