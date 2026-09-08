// Copyright ©2026 Scott Blomfield

using System;

namespace RustArchon.Shared.DTOs;

/// <summary>
/// One Organization that signed up inside the requested window, and whether it ever did anything.
/// </summary>
/// <remarks>
/// A signup on its own is not growth. An account that never provisions a server never connects to
/// anything, never generates load, and churns without ever having been a customer - so the question
/// this report exists to answer is not "how many registered" but "how many started". That is what
/// <see cref="ActivatedOn"/> and <see cref="DaysToActivate"/> carry, and why the summary counts them
/// separately from the signups themselves.
/// </remarks>
public class NewSignupRowDto
{
    public Guid TenantId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public DateTimeOffset SignedUpOn { get; set; }

    /// <summary>Whole days since signup, as of when the report ran.</summary>
    public int DaysSinceSignup { get; set; }

    public string PlanName { get; set; } = string.Empty;
    public string PlanColorCode { get; set; } = string.Empty;
    public int TermMonths { get; set; }

    public int ServerCount { get; set; }

    /// <summary>
    /// When this Organization's first server was added, or <c>null</c> if it still has none - the
    /// moment a signup became a user. See this class's remarks.
    /// </summary>
    public DateTimeOffset? ActivatedOn { get; set; }

    /// <summary>Whole days from signup to first server, or <c>null</c> if never activated.</summary>
    public int? DaysToActivate { get; set; }

    /// <summary>False for an Organization that has since been deactivated.</summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// One live subscription - every Organization currently on a plan, with what it is worth per month.
/// </summary>
/// <remarks>
/// The register behind the plan mix. Deliberately one row per Organization rather than one per plan:
/// an aggregate can be computed from rows, but rows cannot be recovered from an aggregate, and the
/// question after "how many are on Metal" is always "which ones". The plan-level totals are in the
/// summary strip and the plan filter narrows the list.
/// </remarks>
public class SubscriptionRegisterRowDto
{
    public Guid TenantId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string PlanColorCode { get; set; } = string.Empty;

    public int TermMonths { get; set; }

    /// <summary>When this Organization moved onto its current plan - however many periods ago.</summary>
    public DateTimeOffset PlanSince { get; set; }

    /// <summary>Whole days on the current plan.</summary>
    public int DaysOnPlan { get; set; }

    public DateTimeOffset PeriodEnd { get; set; }

    public int ServerCount { get; set; }

    /// <summary>
    /// Server slots bought. What <see cref="MonthlyValue"/> is priced against - a tenant holding four
    /// slots and using two pays for four.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>Ceiling on servers, or <c>null</c> when the plan sells capacity per unit instead.</summary>
    public int? MaximumServers { get; set; }

    /// <summary>What the current billing period was charged.</summary>
    public decimal PeriodAmount { get; set; }

    /// <summary>
    /// The subscription's value per month, from the current catalog price for its plan and term at its
    /// server count. This is the figure the summary's MRR adds up.
    /// </summary>
    /// <remarks>
    /// Derived from list price rather than from <see cref="PeriodAmount"/> divided by the term, because
    /// a period that a mid-period change split carries a prorated amount that would understate the
    /// subscription's actual run rate. Zero when the plan has no price for the term - see
    /// <see cref="MonthlyValueUnknown"/>.
    /// </remarks>
    public decimal MonthlyValue { get; set; }

    /// <summary>True when the plan carries no price for this term, so <see cref="MonthlyValue"/> isn't
    /// a real figure and is left out of the MRR total.</summary>
    public bool MonthlyValueUnknown { get; set; }

    /// <summary>True when a plan or term change is already queued against this subscription.</summary>
    public bool HasScheduledChange { get; set; }

    public bool IsActive { get; set; }
}

/// <summary>Which way a plan move went, priced from the catalog rather than from what anyone paid.</summary>
public enum PlanChangeDirection
{
    /// <summary>Neither plan carries a comparable price - usually a catalog gap.</summary>
    Unknown = 0,
    Upgrade,
    Downgrade,

    /// <summary>Same monthly rate on both sides - a sideways move between equally-priced plans.</summary>
    Lateral
}

/// <summary>
/// One move from one plan to another, taken from the Organization's subscription history.
/// </summary>
/// <remarks>
/// <para>
/// Built from <c>Subscription</c> intervals, which are opened and closed by plan moves - so this covers
/// plan changes, and <strong>not</strong> term changes. A tenant switching monthly to annual on the same
/// plan writes no new interval and does not appear here; that shows up in Scheduled Plan Changes before
/// it lands, and in the Organization's own billing history after.
/// </para>
/// <para>
/// A move onto a zero-priced plan is what churn looks like in this data model. There is no supported
/// "no current plan" state - every Organization always has exactly one open interval - so an account
/// going quiet is a downgrade to the free tier rather than a subscription ending, and
/// <see cref="ToIsFree"/> is what a churn count keys on.
/// </para>
/// </remarks>
public class PlanChangeRowDto
{
    public Guid TenantId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public DateTimeOffset ChangedOn { get; set; }

    public string FromPlanName { get; set; } = string.Empty;
    public string FromPlanColorCode { get; set; } = string.Empty;

    public string ToPlanName { get; set; } = string.Empty;
    public string ToPlanColorCode { get; set; } = string.Empty;

    public PlanChangeDirection Direction { get; set; }

    /// <summary>
    /// The change in monthly list rate, at this Organization's current server count. Positive for an
    /// upgrade.
    /// </summary>
    /// <remarks>
    /// List price at today's catalog and today's server count, not a replay of what was actually
    /// charged at the time - the plan prices in force back then may since have been superseded. It is a
    /// comparison of the two plans, which is what makes the column comparable down the page.
    /// </remarks>
    public decimal MonthlyDelta { get; set; }

    /// <summary>True when the plan moved to costs nothing - see this class's remarks on churn.</summary>
    public bool ToIsFree { get; set; }

    /// <summary>True when the plan moved from cost nothing, i.e. this is a first conversion to paid.</summary>
    public bool FromIsFree { get; set; }
}

/// <summary>
/// One plan or term change an Organization has accepted that hasn't taken effect yet.
/// </summary>
/// <remarks>
/// Committed revenue changes with a future date on them. Worth its own report because the whole point
/// of a deferred change is that nothing about the current subscription reflects it - a downgrade
/// accepted today shows up nowhere else until the day it lands, which may be eleven months away.
/// </remarks>
public class ScheduledChangeRowDto
{
    public Guid TenantId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }

    public string CurrentPlanName { get; set; } = string.Empty;
    public string CurrentPlanColorCode { get; set; } = string.Empty;
    public int CurrentTermMonths { get; set; }

    public string ScheduledPlanName { get; set; } = string.Empty;
    public string ScheduledPlanColorCode { get; set; } = string.Empty;
    public int ScheduledTermMonths { get; set; }

    /// <summary>When the Organization accepted this change.</summary>
    public DateTimeOffset AcceptedOn { get; set; }

    /// <summary>When it takes effect - the end of the billing period in force when it was accepted.</summary>
    public DateTimeOffset EffectiveDate { get; set; }

    /// <summary>Whole days until it lands. Negative if the scheduler hasn't applied it yet.</summary>
    public int DaysUntilEffective { get; set; }

    /// <summary>Change in monthly list rate once it lands. Negative for a downgrade.</summary>
    public decimal MonthlyDelta { get; set; }

    /// <summary>True when the plan being moved to costs nothing.</summary>
    public bool ScheduledIsFree { get; set; }
}
