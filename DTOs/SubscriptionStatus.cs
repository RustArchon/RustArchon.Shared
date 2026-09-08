// Copyright ©2026 Scott Blomfield

namespace RustArchon.Shared.DTOs;

/// <summary>
/// Where a subscription stands in its lifecycle - and, once enforcement exists, whether its servers
/// keep running.
/// </summary>
/// <remarks>
/// <para>
/// <c>Active → PastDue → Suspended → Cancelled</c>, with recovery back to <see cref="Active"/> from
/// either of the first two. This is the one field in the whole billing subsystem that gates access to
/// the product, which is why it lives on the subscription rather than being derived from invoices: a
/// dunning process moves it deliberately, and the Worker acts on the value rather than re-deriving a
/// judgement about someone's payment history for itself.
/// </para>
/// <para>
/// <strong>Nothing moves it yet.</strong> Every subscription is <see cref="Active"/> until dunning and
/// suspension enforcement are built - the field exists now so the states are settled before anything
/// depends on them, not because anything reads them today.
/// </para>
/// </remarks>
public enum SubscriptionStatus
{
    /// <summary>Paid up, or at least not known otherwise. Full access.</summary>
    Active = 0,

    /// <summary>An invoice is past its due date. Access continues - this is the grace period.</summary>
    PastDue = 1,

    /// <summary>Access withdrawn for non-payment. Recoverable: paying moves it back to
    /// <see cref="Active"/>.</summary>
    Suspended = 2,

    /// <summary>Ended deliberately. Terminal - a returning customer gets a new subscription rather than
    /// this one reopening, so the history of what they had before stays intact.</summary>
    Cancelled = 3
}
