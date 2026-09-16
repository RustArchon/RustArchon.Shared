// Copyright ©2026 Scott Blomfield

using System;
using System.Collections.Generic;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// A request to move a tenant onto <see cref="PlanId"/> billed at <see cref="Term"/>. The same shape is
/// used to preview a change and to apply it - previewing is exactly "what would applying this do?", so
/// the two can never drift apart.
/// </summary>
public class ChangePlanRequestDto
{
    public Guid PlanId { get; set; }

    public int TermMonths { get; set; }

    /// <summary>
    /// How many server slots to hold, or <c>null</c> to let the server decide.
    /// </summary>
    /// <remarks>
    /// Null is the normal case for a plan or term change: the API lands entitlement at
    /// <c>max(servers running, the target's included units)</c>, capped at the target's ceiling, so
    /// nobody loses a running server to a plan move and nobody silently keeps paying for slots the new
    /// plan already includes. A value is sent only when capacity itself is what the user is changing.
    /// </remarks>
    public int? Quantity { get; set; }
}

/// <summary>
/// Everything that would happen if a given <see cref="ChangePlanRequestDto"/> were applied - what
/// changes today, what changes later, what it costs, and whether it's allowed at all.
/// </summary>
/// <remarks>
/// <para>
/// Exists because a plan change is <strong>not</strong> necessarily one event. Plan and term are
/// decided independently: whichever dimension costs more takes effect immediately (prorated), and
/// whichever costs less waits for the end of the current term. A single request can therefore do
/// something today <em>and</em> something else months from now - moving to a bigger plan straight away
/// while the shorter term it was asked for only kicks in at renewal, for instance.
/// </para>
/// <para>
/// That delay is the whole reason this type is as explicit as it is. The user has to see, before
/// accepting, that part of what they asked for lands on a date that may be a long way off - so the
/// dates and plan names are carried as real fields (not baked into a prose blob), and
/// <see cref="Effects"/> carries the same thing again as ready-to-display sentences so every client
/// describes it identically.
/// </para>
/// </remarks>
public class PlanChangeQuoteDto
{
    /// <summary>Whether this change may proceed at all. When <c>false</c>, <see cref="BlockedReason"/>
    /// says why and nothing else on this quote should be acted on.</summary>
    public bool Allowed { get; set; }

    /// <summary>User-facing explanation when <see cref="Allowed"/> is <c>false</c> - e.g. the target
    /// plan doesn't allow as many servers as the tenant currently has.</summary>
    public string? BlockedReason { get; set; }

    /// <summary>True when the requested plan and term are what the tenant already has (including a
    /// change already scheduled), so there is nothing to do.</summary>
    public bool IsNoOp { get; set; }

    public string CurrentPlanName { get; set; } = string.Empty;
    public int CurrentTermMonths { get; set; }

    /// <summary>Server slots held right now - see <c>SubscriptionPeriod.Quantity</c>.</summary>
    public int CurrentQuantity { get; set; }

    /// <summary>End of the billing period in force right now - the date deferred parts of the change
    /// wait for.</summary>
    public DateTimeOffset CurrentPeriodEnd { get; set; }

    /// <summary>Whether any part of this change takes effect immediately.</summary>
    public bool HasImmediateChange { get; set; }

    public string? ImmediatePlanName { get; set; }
    public int? ImmediateTermMonths { get; set; }

    /// <summary>Slots held straight after this change - higher than <see cref="CurrentQuantity"/> only
    /// when capacity was bought, since releasing it is deferred like any other reduction.</summary>
    public int? ImmediateQuantity { get; set; }

    /// <summary>The prorated amount owed for the immediate part - see
    /// <c>PlanChangeCalculator</c> for the formula. Zero when nothing applies immediately. Downgrades
    /// never produce a refund, so this is never negative.</summary>
    public decimal AmountDueNow { get; set; }

    /// <summary>Where the current period ends once the immediate part is applied. Differs from
    /// <see cref="CurrentPeriodEnd"/> only when the term got longer - the period keeps its original
    /// start date and simply runs further out.</summary>
    public DateTimeOffset PeriodEndAfterChange { get; set; }

    /// <summary>Whether some part of this change is deferred to a future date.</summary>
    public bool HasScheduledChange { get; set; }

    /// <summary>
    /// Whether accepting this change will collapse the organization onto the single built-in Owner
    /// role, because the target plan does not include role separation.
    /// </summary>
    /// <remarks>
    /// The warning half of the compression decision. Everyone keeps their access - they gain some -
    /// but the distinctions the customer drew between their people disappear, and on the new plan
    /// every member can change the plan and spend money. That is a consequence worth stating in
    /// advance rather than discovering afterwards, which is why it rides on the quote.
    /// </remarks>
    public bool WillCompressRoles { get; set; }

    /// <summary>How many members would be promoted to Owner. See <see cref="WillCompressRoles"/>.</summary>
    /// <remarks>
    /// A forecast, not a promise: a deferred downgrade lands weeks later and the membership will have
    /// moved on by then, so the number applied is re-derived at that point.
    /// </remarks>
    public int MembersToPromote { get; set; }

    /// <summary>How many of the organization's own roles would be retired.</summary>
    public int RolesToRemove { get; set; }

    public string? ScheduledPlanName { get; set; }
    public int? ScheduledTermMonths { get; set; }

    /// <summary>Slots the deferred part lands on - set when capacity is being released.</summary>
    public int? ScheduledQuantity { get; set; }

    /// <summary>When the deferred part takes effect. Note this is
    /// <see cref="PeriodEndAfterChange"/>, not <see cref="CurrentPeriodEnd"/> - extending the term as
    /// part of the same request pushes the deferred part out with it, which is exactly the kind of
    /// surprise this quote exists to make visible up front.</summary>
    public DateTimeOffset? ScheduledEffectiveDate { get; set; }

    /// <summary>Plain-English sentences describing every consequence of this change, in the order they
    /// happen. Rendered as-is by clients so the wording is identical everywhere.</summary>
    public List<string> Effects { get; set; } = [];
}

/// <summary>
/// A tenant's current subscription plus any change already queued against it - what the "your plan"
/// screen renders before the user asks for anything.
/// </summary>
public class SubscriptionDto
{
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string PlanColorCode { get; set; } = string.Empty;
    public int TermMonths { get; set; }

    /// <summary>
    /// Where the subscription stands - see <see cref="SubscriptionStatus"/>. What
    /// <c>SubscriptionStatusBanner</c> reads to decide whether to show a "you're past due"/"your
    /// servers are suspended" notice.
    /// </summary>
    public SubscriptionStatus Status { get; set; }

    /// <summary>Why <see cref="Status"/> last changed, in the words of whoever changed it - shown
    /// alongside the banner above, when set.</summary>
    public string? StatusReason { get; set; }

    /// <summary>When this tenant moved onto their current plan - however many billing periods ago. Not
    /// the same as <see cref="PeriodStart"/>, which is only the current period.</summary>
    public DateTimeOffset PlanSince { get; set; }

    public DateTimeOffset PeriodStart { get; set; }
    public DateTimeOffset PeriodEnd { get; set; }

    /// <summary>What this period was priced at. Until checkout exists this is the plan's list price for
    /// the term, not a record of money actually collected - see <c>Subscription.PeriodAmount</c>.</summary>
    public decimal PeriodAmount { get; set; }

    /// <summary>Ceiling on servers, or <c>null</c> when the plan has none and capacity is bought instead.</summary>
    public int? MaximumServers { get; set; }

    public int CurrentServerCount { get; set; }

    /// <summary>
    /// Server slots currently paid for. Servers consume these; adding one needs a spare.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Whether the user may buy or release slots on this plan.
    /// </summary>
    /// <remarks>
    /// False on a flat tier, where quantity is pinned to the price row's included units: with a unit
    /// amount of zero, buying slots would charge nothing and mean nothing, so the control isn't offered
    /// at all rather than offered and ignored.
    /// </remarks>
    public bool CanChangeQuantity { get; set; }

    /// <summary>Slots the current plan and term include before any are bought.</summary>
    public int IncludedUnits { get; set; }

    /// <summary>What one extra slot costs for a whole period at the current term. Zero on a flat tier.</summary>
    public decimal UnitAmount { get; set; }

    /// <summary>
    /// The fewest slots this tenant may hold: they can't release capacity a running server is using.
    /// </summary>
    public int MinimumQuantity { get; set; }

    /// <summary>A change already accepted but not yet in force - the deferred half of an earlier
    /// request. Null when nothing is queued.</summary>
    public ScheduledPlanChangeDto? ScheduledChange { get; set; }
}

/// <summary>A plan/term change the tenant has accepted that takes effect on a future date.</summary>
public class ScheduledPlanChangeDto
{
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public int TermMonths { get; set; }

    /// <summary>Slots the subscription drops to when this lands.</summary>
    public int Quantity { get; set; }

    public DateTimeOffset EffectiveDate { get; set; }
}

/// <summary>
/// One billable span from a tenant's billing history - what they were charged, for which dates, on
/// which plan.
/// </summary>
/// <remarks>
/// Usually one row per billing period, but a change landing mid-period splits that period in two: the
/// span up to the change at the old plan's rate, and the span from the change to renewal at the new
/// one. <see cref="IsWholePeriod"/> distinguishes them, so a statement can mark the partial rows as
/// prorated rather than leaving a reader to wonder why one month cost $1.61.
/// </remarks>
public class BillingHistoryEntryDto
{
    public string PlanName { get; set; } = string.Empty;
    public int TermMonths { get; set; }

    /// <summary>Server slots this span was billed for.</summary>
    public int Quantity { get; set; }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public decimal Amount { get; set; }

    /// <summary>False when this row covers only part of its billing period - i.e. it's a prorated
    /// slice created by a mid-period change.</summary>
    public bool IsWholePeriod { get; set; }

    // Invoice state, read from the document that bills this period rather than from stamps on the
    // period itself. That distinction is why these are nullable: a period that hasn't been invoiced
    // (a free plan, which raises no document at all) legitimately has none of it.

    /// <summary>The invoice covering this period, or <c>null</c> if none was raised.</summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// The invoice's own id, or <c>null</c> if none was raised - what a "Pay now" control needs to name
    /// which invoice to check out, since <see cref="InvoiceNumber"/> alone isn't a usable key.
    /// </summary>
    public Guid? InvoiceId { get; set; }

    public DateTimeOffset? InvoicedOn { get; set; }
    public DateTimeOffset? DueOn { get; set; }

    /// <summary>The invoice's status, or <c>null</c> when there is no invoice.</summary>
    public InvoiceStatus? InvoiceStatus { get; set; }

    /// <summary>Still owed on that invoice. Zero once settled.</summary>
    public decimal Outstanding { get; set; }
}

/// <summary>
/// A Plan a tenant may move to, as offered on the change-plan screen.
/// </summary>
/// <remarks>
/// Neither of the two existing Plan shapes fits here. <c>PlanDto</c> is the platform admin's view -
/// audit fields and <c>SubscriberCount</c>, behind the <c>ManagePlans</c> permission an ordinary member
/// doesn't have. <c>PublicPlanDto</c> is the anonymous marketing view and deliberately carries no
/// <c>Id</c>, which a change request needs to name its target. This is the third audience: a signed-in
/// member choosing among what's on sale.
/// </remarks>
public class PlanOptionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;

    /// <summary>The terms this plan is offered on, and what each costs.</summary>
    public List<PlanPriceDto> Prices { get; set; } = [];

    public PricingModel PricingModel { get; set; }

    /// <summary>Null when the plan has no ceiling - capacity is bought per unit.</summary>
    public int? MaximumServers { get; set; }

    public int MaximumUsers { get; set; }
    public int RetentionHistory { get; set; }
    public bool HasRoles { get; set; }

    /// <summary>
    /// Whether one person may only have a single Organization of their own on this plan.
    /// </summary>
    /// <remarks>
    /// Carried onto the change-plan screen so it can explain a refusal before it happens - somebody
    /// with two Organizations who tries to move the second onto the free tier is turned away, and
    /// finding that out on submit is worse than seeing it on the card.
    /// </remarks>
    public bool OnePerOwner { get; set; }

    /// <summary>True for the plan the tenant is on right now.</summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// False when this plan allows fewer servers than the tenant currently has - the rule that a
    /// change may never leave an Organization over its limit. The screen disables these rather than
    /// letting someone pick one and be refused on submit.
    /// </summary>
    public bool AllowedForCurrentServerCount { get; set; }
}

